using FlatBuffers;
using Photon.Pun;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class GamePhoton : PhotonMessenger
{
	Game game => Game.Instance;

	protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Game_PlayerLoadedScene:
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

			//case EPhotonMsg.Game_Activate:
			//	game.Activate();
			//	break;

			case EPhotonMsg.Game_Ui_ShowTimeValue:
				int timeValue = (int)pParams[0];
				game.UIGameTime.ShowTimeValue(timeValue);
				break;

			case EPhotonMsg.Game_UpdatePlayerScore:
				int playerNumber = (int)pParams[0];
				int kills = (int)pParams[1];
				int deaths = (int)pParams[2];
				game.Results.UpdatePlayerScore(playerNumber, kills, deaths);
				break;

			case EPhotonMsg.Game_EndGame:

				GameResultInfoS infoS = GameResultInfoS.GetRootAsGameResultInfoS(bb);
				GameResultInfo gameResultInfo = GameResultInfo.Deserialize(infoS);
				Debug.Log("Game_SetGameResultInfo");
				Debug.Log(gameResultInfo);

				game.GameEnd.OnReceiveEndGame(gameResultInfo);
				break;
			default:
				Debug.LogError("Message not handled");
				break;
		}
	}

	protected override bool CanSend(EPhotonMsg pMsgType)
	{
		bool isMaster = brainiacs.PhotonManager.IsMaster();
		switch(pMsgType)
		{
			//master keeps all player results, no need to send to clients
			case EPhotonMsg.Game_UpdatePlayerScore:
				return !isMaster;
			//only master send time 
			case EPhotonMsg.Game_Ui_ShowTimeValue:
				return isMaster;
		}

		return true;
	}

	protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
	}
}
