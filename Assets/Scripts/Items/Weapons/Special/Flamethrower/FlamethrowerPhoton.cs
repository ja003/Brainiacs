using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerPhoton : SpecialWeaponPhoton
{

	protected override void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		FlamethrowerController flamethrower = (FlamethrowerController)controller;

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
