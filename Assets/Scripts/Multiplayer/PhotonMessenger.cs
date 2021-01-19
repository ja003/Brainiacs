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
[RequireComponent(typeof(PhotonView))]
public abstract class PhotonMessenger : BrainiacsBehaviour
{
	[Header("Photon")]
	//[SerializeField] 
	private PhotonView _view = null;
	public PhotonView view
	{
		get
		{
			if(_view == null)
				_view = GetComponent<PhotonView>();
			if(!_view)
				Debug.LogError("PhotonView not found");
			return _view;
		}
	}

	protected virtual bool IsLocalImage() { return false; }

	public bool IsMine => view && isMultiplayer ? view.IsMine : !IsLocalImage();
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

	/// <summary>
	/// Handles network message - implementation in subclasses.
	/// Note: right now all messages has to have some pParams.
	/// If needed implement other method with no pParams.
	/// </summary>
	[PunRPC]
	public void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams)
	{
		//var receivedMsg = (EPhotonMsg_MainMenu)objectArray[0];
		//if(DEBUG_LOG && !IsLogMsgIgnored(pReceivedMsg))
		//	Debug.Log($"{gameObject.name} HandleMsg {pReceivedMsg}");

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
			case EPhotonMsg.Game_EndGame:
				return true;
		}

		return false;
	}

	protected abstract void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb);

	protected void OnMsgUnhandled(EPhotonMsg pReceivedMsg)
	{
		Debug.LogError(this.gameObject.name + " message not handled " + pReceivedMsg);
	}

	public virtual void Send(EPhotonMsg pMsgType, params object[] pParams)
	{
		Send(pMsgType, null, pParams);
	}

	/// <summary>
	/// Send data to {target} (based on message type).
	/// If game is not multiplayer the message wont send 
	/// and will be handled right away
	/// </summary>
	public void Send(EPhotonMsg pMsgType, PhotonPlayer pTargetPlayer, params object[] pParams)
	{
		//if(DEBUG_LOG)
		//	Debug.Log($"{gameObject.name} Send {pMsgType}");

		//todo: add timestamp to all messages?
		//eg. neede for setting health


		if(!CanSend(pMsgType))
		{
			if(DEBUG_LOG && !IsLogMsgIgnored(pMsgType))
				Debug.Log(gameObject.name + " - cant send message: " + pMsgType);
			return;
		}

		if(isMultiplayer)
		{
			if(!PhotonNetwork.IsConnected)
			{
				Debug.Log("Not connected - cant send message");
				return;
			}
			if(DEBUG_LOG && !IsLogMsgIgnored(pMsgType))
				Debug.Log("Send " + pMsgType);

			RpcTarget target = GetMsgTarget(pMsgType);
			//view.RPC("HandleMsg", target, pMsgType, pParams);
			//https://forum.photonengine.com/discussion/16375/photon-unity-send-an-rpc-function-to-one-specific-player-to-execute
			//this doesnt work?
			//msg is sent to all, not just pTargetPlayer
			// => handled in PlayerPhoton.HandleMsg2
			if(pTargetPlayer == null)
				view.RPC("HandleMsg", target, pMsgType, pParams);
			else
				view.RPC("HandleMsg", pTargetPlayer, pMsgType, pParams);

		}
		else
		{
			if(DEBUG_LOG && !IsLogMsgIgnored(pMsgType))
				Debug.Log("SendNotMP " + pMsgType);

			if(debug_notMpMsgDelay > 0)
				DoInTime(() => debug_SendNotMP(pMsgType, pParams), debug_notMpMsgDelay);
			else
				debug_SendNotMP(pMsgType, pParams);
		}
	}

	[SerializeField] float debug_notMpMsgDelay = 0.1f;

	/// <summary>
	/// Debug for clearer output. 
	/// Skip too frequent and unnecessary messages
	/// </summary>
	private bool IsLogMsgIgnored(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Pool_SetActive:
				return !IsMine;

			case EPhotonMsg.Player_ShowWeapon:
			case EPhotonMsg.Player_ChangeDirection:
			case EPhotonMsg.Player_UI_PlayerInfo_SetActiveWeapon:
			case EPhotonMsg.Player_UI_PlayerInfo_SetAmmo:
			case EPhotonMsg.Player_UI_PlayerInfo_SetHealth:
			case EPhotonMsg.Player_UI_Scoreboard_SetScore:
				return true;
		}

		return false;
	}

	protected abstract void debug_SendNotMP(EPhotonMsg pMsgType, object[] pParams);

	protected abstract bool CanSend(EPhotonMsg pMsgType);

	private RpcTarget GetMsgTarget(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Game_PlayerLoadedScene:
			case EPhotonMsg.Game_UpdatePlayerScore: //only master keeps all player scores
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

	//Pool
	Pool_SetActive,
	Pool_ReturnToPool,

	//Game
	Game_PlayerLoadedScene,
	//Game_Activate, //no need, activate is called autimatically on all sides
	Game_UpdatePlayerScore,
	Game_EndGame,

	Game_Ui_ShowTimeValue,

	Game_HandleEffect,

	Game_Lighting_SetMode,

	//Main menu
	MainMenu_SyncGameInfo,
	MainMenu_SyncPlayerInfo,


	//Player
	Player_ChangeDirection,
	Player_InitPlayer,
	Player_ShowWeapon,
	Player_ApplyDamage,
	Player_AddKill,
	Player_Push,

	Player_OnReceiveDamageEffect, //visual + sound
	Player_Visual_OnDie,
	Player_DoEliminateEffect,

	Player_PlayWeaponUseSound, //sound
	Player_DoShieldHitEffect,

	Player_SetSyncPosition,

	Player_UI_PlayerInfo_SetHealth,
	Player_UI_PlayerInfo_SetReloading,
	Player_UI_PlayerInfo_SetAmmo,
	Player_UI_PlayerInfo_SetActiveWeapon,

	Player_UI_Scoreboard_SetScore,

	Player_UI_SetEffectActive,


	//Projectile
	Projectile_Spawn,
	// - projectile is Destroyed using PhotonNetwork.Destroy => no need for message

	//Special
	Special_Init,
	//Special_Use,
	//Special_StopUse,

	//- Flamethrower
	Special_Flamethrower_OnDirectionChange,

	//- Curie
	Special_Curie_StartTruck,
	Special_Curie_Collide,

	//- Einstein
	Special_Einstein_FallOn,

	//- Nobel
	Special_Nobel_Spawn,
	Special_Nobel_Explode,

	//- DaVinci
	Special_DaVinci_OnCollision,
	Special_DaVinci_UpdateHealthbar,

	//Map items
	MapItem_Init,

	//MapItem_InitMapSpecial,
	//MapItem_InitMapBasic,
	//MapItem_InitPowerUp,
	MapItem_DoExplosionEffect,
	//MapItem_ReturnToPool,

	//Map
	Map_Obstackle_DoCollisionEffect,
}