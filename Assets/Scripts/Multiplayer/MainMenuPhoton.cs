using FlatBuffers;
using Photon.Pun;
using UnityEngine;

public class MainMenuPhoton : MainMenuController
{
	[SerializeField] PhotonView view;

	protected override void OnMainControllerAwaken()
	{
	}

	public void debug_HandleMsg(EPhotonMsg_MainMenu receivedMsg, object pParam)
	{
		HandleMsg(receivedMsg, new object[] { pParam });
	}


	[PunRPC]
	//public void HandleMsg(object[] objectArray)
	public void HandleMsg(EPhotonMsg_MainMenu receivedMsg, object[] pParams)
	{
		//var receivedMsg = (EPhotonMsg_MainMenu)objectArray[0];
		Debug.Log("HandleMsg " + receivedMsg);

		byte[] bytes = pParams != null ? (byte[])pParams[0] : null;
		ByteBuffer bb = bytes != null ? new ByteBuffer(bytes) : null;

		switch(receivedMsg)
		{
			case EPhotonMsg_MainMenu.Play:
				brainiacs.Scenes.LoadScene(EScene.Loading);
				break;
			case EPhotonMsg_MainMenu.SyncGameInfo:

				GameInitInfoS infoS = GameInitInfoS.GetRootAsGameInitInfoS(bb);
				GameInitInfo gameInfo = GameInitInfo.Deserialize(infoS);
				Debug.Log(gameInfo);

				//reset current game info - DONT assign it
				//it will be set from UI elements
				brainiacs.GameInitInfo = new GameInitInfo();
				mainMenu.GameSetup.OpenMain(gameInfo);
				break;

			case EPhotonMsg_MainMenu.SyncPlayerInfo:
				PlayerInitInfoS playerInfoS = PlayerInitInfoS.GetRootAsPlayerInitInfoS(bb);
				PlayerInitInfo playerInfo = PlayerInitInfo.Deserialize(playerInfoS);
				mainMenu.GameSetup.SetupMain.UpdatePlayer(playerInfo);
				break;

			case EPhotonMsg_MainMenu.None:
			default:
				Debug.LogError("Message not handled");

				break;
		}
	}

	public void Send(EPhotonMsg_MainMenu pMsgType, object pParams)
	{
		Send(pMsgType, new object[] { pParams });
	}

	/// <summary>
	/// Send data to {target} (based on message type).
	/// If game is not multiplayer the message wont send 
	/// and will be handled right away
	/// </summary>
	public void Send(EPhotonMsg_MainMenu pMsgType, object[] pParams = null)
	{
		bool isMultiplayer = brainiacs.GameInitInfo.IsMultiplayer();
		//isMultiplayer = true; //debug

		Debug.Log("Send " + pMsgType + ", MP: " + isMultiplayer);

		if(isMultiplayer)
		{
			if(!PhotonNetwork.IsConnected)
			{
				Debug.Log("Not connected - cant send message");
				return;
			}

			RpcTarget target = GetMsgTarget(pMsgType);
			view.RPC("HandleMsg", target, pMsgType, pParams);
		}
		else
		{
			//debug
			HandleMsg(pMsgType, pParams);
		}
	}

	private RpcTarget GetMsgTarget(EPhotonMsg_MainMenu pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg_MainMenu.Play:
				return RpcTarget.All;
			case EPhotonMsg_MainMenu.SyncGameInfo:
				//return RpcTarget.AllViaServer; //debug
				return RpcTarget.Others;
		}
		return RpcTarget.Others;
	}
}

public enum EPhotonMsg_MainMenu
{
	None = 0,
	Play = 1,
	SyncGameInfo = 2,
	SyncPlayerInfo = 3
}