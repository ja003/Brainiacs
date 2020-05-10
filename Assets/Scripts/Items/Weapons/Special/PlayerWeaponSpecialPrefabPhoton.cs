using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPlayer = Photon.Realtime.Player;

public abstract class PlayerWeaponSpecialPrefabPhoton : PoolObjectPhoton
{
	//[SerializeField] protected PlayerWeaponSpecialController controller;

	private PlayerWeaponSpecialPrefab _prefab;
	protected PlayerWeaponSpecialPrefab prefab
	{
		get
		{
			if(_prefab == null)
				_prefab = GetComponent<PlayerWeaponSpecialPrefab>();
			return _prefab;
		}
	}

	sealed protected override bool CanSend2(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Special_Init:
				return IsMine;
		}

		return CanSend3(pMsgType);
	}

	protected abstract bool CanSend3(EPhotonMsg pMsgType);


	sealed protected override void HandleMsg2(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Special_Init:
				int playerNumber = (int)pParams[0];
				Game.Instance.PlayerManager.OnAllPlayersAdded.AddAction(() => InitPrefab(playerNumber));

				//todo: maybe no need to send playerNumber, get it from view.OwnerActorNr

				break;

			//case EPhotonMsg.Special_Use:
			//	controller.Use();
			//	break;
			//case EPhotonMsg.Special_StopUse:
			//	controller.StopUse();
			//	break;
			default:
				HandleMsg3(pReceivedMsg, pParams, bb);
				break;
		}
	}

	protected abstract void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb);
	//{
	//	OnMsgUnhandled(pReceivedMsg);
	//}

	private void InitPrefab(int pPlayerNumber)
	{
		if(DEBUG_LOG)
			Debug.Log($"P({pPlayerNumber}) InitController {prefab.name}");
		Player player = Game.Instance.PlayerManager.GetPlayer(pPlayerNumber);
		prefab.Init(player);
	}

	//protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	//{
	//	if(controller._LocalImage)
	//	{
	//		controller._LocalImage.Photon.HandleMsg(pMsgType, pParams);
	//	}
	//}

	//protected abstract PhotonMessenger GetLocalImageNetwork();
}
