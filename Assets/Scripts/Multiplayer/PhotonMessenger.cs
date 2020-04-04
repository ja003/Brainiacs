using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

/// <summary>
/// Handles RPC messanging of one photon view
/// </summary>
public abstract class PhotonMessenger : BrainiacsBehaviour
{
	//protected PhotonView view { get; private set; }

	private PhotonView _view;
	protected PhotonView view
	{
		get
		{
			if(_view == null)
				_view = GetComponent<PhotonView>();
			return _view;
		}
	}

	public bool IsMine => view ? view.IsMine : true;
	public PhotonPlayer PhotonController => view.Controller;

	[SerializeField] protected bool DEBUG_LOG = false;

	//protected override void Awake()
	//{
	//	view = GetComponent<PhotonView>();
	//	base.Awake();
	//}

	public void debug_HandleMsg(EPhotonMsg pReceivedMsg, object pParam)
	{
		HandleMsg(pReceivedMsg, new object[] { pParam });
	}

	[PunRPC]
	public void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams)
	{
		//var receivedMsg = (EPhotonMsg_MainMenu)objectArray[0];
		if(DEBUG_LOG)
			Debug.Log("HandleMsg " + pReceivedMsg);

		byte[] bytes = IsFlafbuffer(pReceivedMsg) && pParams != null ? (byte[])pParams[0] : null;
		ByteBuffer bb = IsFlafbuffer(pReceivedMsg) && bytes != null ? new ByteBuffer(bytes) : null;

		HandleMsg(pReceivedMsg, pParams, bb);
	}

	private bool IsFlafbuffer(EPhotonMsg pMessage)
	{
		switch(pMessage)
		{
			case EPhotonMsg.MainMenu_SyncGameInfo:
			case EPhotonMsg.MainMenu_SyncPlayerInfo:
				return true;
		}

		return false;
	}

	protected abstract void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb);

	/// <summary>
	/// Send data to {target} (based on message type).
	/// If game is not multiplayer the message wont send 
	/// and will be handled right away
	/// </summary>
	public void Send(EPhotonMsg pMsgType, params object[] pParams)
	{
		bool isMultiplayer = brainiacs.GameInitInfo.IsMultiplayer();
		
		
		//if(DEBUG_LOG)
		//	Debug.Log("Send " + pMsgType + ", MP: " + isMultiplayer);

		//todo: add timestamp to all messages?
		//eg. neede for setting health

		//only MINE player can send messages to others
		//exceptions:
		bool isException =
			//info that client need to init his player
			pMsgType == EPhotonMsg.Player_InitPlayer ||
			pMsgType == EPhotonMsg.Player_ApplyDamage;

		if(!CanSend() && !isException)
		{
			return;
		}

		if(isMultiplayer)
		{
			if(!PhotonNetwork.IsConnected)
			{
				Debug.Log("Not connected - cant send message");
				return;
			}
			if(DEBUG_LOG)
				Debug.Log("Send " + pMsgType);

			RpcTarget target = GetMsgTarget(pMsgType);
			view.RPC("HandleMsg", target, pMsgType, pParams);
		}
		else
		{
			if(DEBUG_LOG)
				Debug.Log("SendNotMP " + pMsgType);

			SendNotMP(pMsgType, pParams);
		}
	}

	protected abstract void SendNotMP(EPhotonMsg pMsgType, object[] pParams);

	protected abstract bool CanSend();

	private RpcTarget GetMsgTarget(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Game_PlayerLoadedScene:
				return RpcTarget.MasterClient;
			//case EPhotonMsg_MainMenu.Play:
			//	return RpcTarget.All;
			case EPhotonMsg.MainMenu_SyncGameInfo:
				//return RpcTarget.AllViaServer; //debug
				return RpcTarget.Others;
		}
		return RpcTarget.Others;
	}

	public override string ToString()
	{
		return $"[{gameObject.name}_{view.ViewID}]";
	}
}

public enum EPhotonMsg
{
	None,

	//Game
	Game_PlayerLoadedScene,

	//Main menu
	MainMenu_SyncGameInfo,
	MainMenu_SyncPlayerInfo,


	//Player
	Player_ChangeDirection,
	Player_InitPlayer,
	Player_ShowWeapon,
	Player_ApplyDamage,

	Player_UI_PlayerInfo_SetHealth,
	Player_UI_PlayerInfo_SetReloading,
	Player_UI_PlayerInfo_SetAmmo,
	Player_UI_PlayerInfo_SetActiveWeapon,

	Player_UI_Scoreboard_SetStats,

	//Projectile
	Projectile_Spawn,
	// - projectile is Destroyed using PhotonNetwork.Destroy => no need for message

	//Special
	Special_Init,
	Special_Use,
	Special_StopUse,

	//- Flamethrower
	Special_Flamethrower_OnDirectionChange,

	//- Curie
	//Special_Curie_StartTruck,
	Special_Curie_Collide,

	//- Einstein
	Special_Einstein_FallOn,
}