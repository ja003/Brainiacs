using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerPhoton : PlayerWeaponSpecialPrefabPhoton
{
	[SerializeField] SpecialFlamethrowerFlame flamethrower = null;

	protected override bool CanSend3(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Special_Flamethrower_OnDirectionChange:
				return IsMine;
		}
		return false;
	}

	protected override void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		//FlamethrowerController flamethrower = (FlamethrowerController)controller;

		switch(pReceivedMsg)
		{
			case EPhotonMsg.Special_Flamethrower_OnDirectionChange:
				EDirection dir = (EDirection)pParams[0];
				flamethrower.OnDirectionChange(dir);
				break;
			default:
				OnMsgUnhandled(pReceivedMsg);
				break;
		}
	}

}
