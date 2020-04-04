using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPlayer = Photon.Realtime.Player;

public class SpecialWeaponNetwork : PhotonMessenger
{
	[SerializeField] protected PlayerWeaponSpecialController controller;

	protected override bool CanSend()
	{
		return view.IsMine || controller._LocalRemote;
	}

	protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Special_Init:
				int playerNumber = (int)pParams[0];
				//PhotonPlayer photonPlayer = null;
				//for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
				//{
				//	if(PhotonNetwork.PlayerList[i].ActorNumber == photonPlayerNumber)
				//		photonPlayer = PhotonNetwork.PlayerList[i];
				//}
				//if(photonPlayer ==null)
				//{
				//	Debug.LogError($"Photon player {photonPlayerNumber} not found");
				//	return;
				//}

				//TODO: implement event trigger controller
				Game.Instance.PlayerManager.OnAllPlayersAdded.AddAction(() => InitController(playerNumber));				
				break;

			case EPhotonMsg.Special_Use:
				controller.Use();
				break;
			case EPhotonMsg.Special_StopUse:
				controller.StopUse();
				break;
		}
	}

	private void InitController(int pPlayerNumber)
	{
		if(DEBUG_LOG)
			Debug.Log($"P({pPlayerNumber}) InitController {controller.name}" );
		Player player = Game.Instance.PlayerManager.GetPlayer(pPlayerNumber);
		controller.Init(player);
	}

	protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
		if(controller._LocalRemote)
		{
			controller._LocalRemote.Network.HandleMsg(pMsgType, pParams);
		}
	}

	//protected abstract PhotonMessenger GetLocalRemoteNetwork();
}
