using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPlayer = Photon.Realtime.Player;

public class SpecialWeaponPhoton : PoolObjectPhoton
{
	//[SerializeField] protected PlayerWeaponSpecialController controller;

	private PlayerWeaponSpecialController _controller;
	protected PlayerWeaponSpecialController controller
	{
		get
		{
			if(_controller == null)
				_controller = GetComponent<PlayerWeaponSpecialController>();
			return _controller;
		}
	}

	//protected override void Awake()
	//{
	//	if(controller == null)
	//		controller = GetComponent<PlayerWeaponSpecialController>();

	//	base.Awake();
	//}

	protected override bool CanSendMsg(EPhotonMsg pMsgType)
	{
		return view.IsMine || controller._LocalImage;
	}

	sealed protected override void HandleMsg2(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Special_Init:
				int playerNumber = (int)pParams[0];
				Game.Instance.PlayerManager.OnAllPlayersAdded.AddAction(() => InitController(playerNumber));
				break;

			case EPhotonMsg.Special_Use:
				controller.Use();
				break;
			case EPhotonMsg.Special_StopUse:
				controller.StopUse();
				break;
			default:
				HandleMsg3(pReceivedMsg, pParams, bb);
				break;
		}
	}

	protected virtual void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		OnMsgUnhandled(pReceivedMsg);
	}

	private void InitController(int pPlayerNumber)
	{
		if(DEBUG_LOG)
			Debug.Log($"P({pPlayerNumber}) InitController {controller.name}");
		Player player = Game.Instance.PlayerManager.GetPlayer(pPlayerNumber);
		controller.Init(player);
	}

	protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
		if(controller._LocalImage)
		{
			controller._LocalImage.Photon.HandleMsg(pMsgType, pParams);
		}
	}

	//protected abstract PhotonMessenger GetLocalImageNetwork();
}
