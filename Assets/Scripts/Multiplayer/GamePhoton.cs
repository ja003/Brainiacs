using FlatBuffers;
using Photon.Pun;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class GamePhoton : GameController
{
	[SerializeField] PhotonView view;

	protected override void OnMainControllerAwaken()
	{
	}

	[PunRPC]
	public void HandleMsg(EPhotonMsg_Game receivedMsg, object[] pParams)
	{
		//var receivedMsg = (EPhotonMsg_MainMenu)objectArray[0];
		//Debug.Log("HandleMsg " + receivedMsg);

		byte[] bytes;// = pParams != null ? (byte[])pParams[0] : null;
		ByteBuffer bb;// = bytes != null ? new ByteBuffer(bytes) : null;

		switch(receivedMsg)
		{
			case EPhotonMsg_Game.PlayerLoadedScene:
				if(!PhotonNetwork.IsMasterClient)
				{
					Debug.LogError("Only master should receive PlayerLoadedScene");
					return;
				}

				PhotonPlayer player = 
					PhotonNetwork.CurrentRoom.GetPlayer((int)pParams[0]);
				Debug.Log(player + " loaded game");
				game.PlayerManager.OnRemotePlayerLoadedScene(player);
				break;
			default:
				Debug.LogError("Message not handled");
				break;
		}
	}

	public void Send(EPhotonMsg_Game pMsgType, object pParams)
	{
		Send(pMsgType, new object[] { pParams });
	}

	public void Send(EPhotonMsg_Game pMsgType, object[] pParams = null)
	{
		bool isMultiplayer = brainiacs.GameInitInfo.IsMultiplayer();
		//isMultiplayer = true; //debug


		if(isMultiplayer)
		{
			if(!PhotonNetwork.IsConnected)
			{
				Debug.Log("Not connected - cant send message");
				return;
			}
			Debug.Log("Send " + pMsgType);

			RpcTarget target = GetMsgTarget(pMsgType);
			view.RPC("HandleMsg", target, pMsgType, pParams);
		}
		else
		{
			//debug
			HandleMsg(pMsgType, pParams);
		}
	}

	private RpcTarget GetMsgTarget(EPhotonMsg_Game pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg_Game.PlayerLoadedScene:
				return RpcTarget.MasterClient;
		}
		return RpcTarget.Others;
	}
}

public enum EPhotonMsg_Game
{
	None = 0,
	PlayerLoadedScene = 1
}