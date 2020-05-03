using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEinsteinPhoton : SpecialWeaponPhoton
{
	protected override void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		SpecialEinstein bomb = (SpecialEinstein)controller;
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Special_Einstein_FallOn:
				Vector3 target = (Vector3)pParams[0];
				bomb.FallOn(target);
				break;
			default:
				OnMsgUnhandled(pReceivedMsg);
				break;
		}
	}

}
