using FlatBuffers;
using Photon.Pun;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class GamePhoton : PhotonMessenger
{

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
				Game.Instance.PlayerManager.OnRemotePlayerLoadedScene(player);
				break;
			default:
				Debug.LogError("Message not handled");
				break;
		}
	}

	protected override bool CanSend()
	{
		return true;
	}

	protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
	}
}
