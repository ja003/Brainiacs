using FlatBuffers;
using Photon.Pun;
using UnityEngine;

public class MainMenuPhoton : PhotonMessenger
{
	MainMenu mainMenu => MainMenu.Instance;

	//public void debug_HandleMsg(EPhotonMsg_MainMenu receivedMsg, object pParam)
	//{
	//	HandleMsg(receivedMsg, new object[] { pParam });
	//}

	protected override bool CanSend(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.MainMenu_SyncGameInfo:
				//Debug.Log("CanSend sync = " + brainiacs.PhotonManager.IsMaster());
				return brainiacs.PhotonManager.IsMaster();
		}

		return true;
	}

	protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.MainMenu_SyncGameInfo:

				GameInitInfoS infoS = GameInitInfoS.GetRootAsGameInitInfoS(bb);
				GameInitInfo gameInfo = GameInitInfo.Deserialize(infoS);
				Debug.Log(gameInfo);

				//reset current game info - DONT assign it
				//it will be set from UI elements
				brainiacs.GameInitInfo = new GameInitInfo();
				mainMenu.GameSetup.OpenMain(gameInfo);
				break;

			case EPhotonMsg.MainMenu_SyncPlayerInfo:
				PlayerInitInfoS playerInfoS = PlayerInitInfoS.GetRootAsPlayerInitInfoS(bb);
				PlayerInitInfo playerInfo = PlayerInitInfo.Deserialize(playerInfoS);
				mainMenu.GameSetup.SetupMain.UpdatePlayer(playerInfo);
				break;

			case EPhotonMsg.None:
			default:
				Debug.LogError("Message not handled");

				break;
		}
	}


	protected override void debug_SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
		//HandleMsg(pMsgType, pParams);
	}

}
