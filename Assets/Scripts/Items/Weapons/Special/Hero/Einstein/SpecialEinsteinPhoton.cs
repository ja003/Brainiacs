using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEinsteinPhoton : PlayerWeaponSpecialPrefabPhoton
{
	[SerializeField] SpecialEinsteinBomb bomb = null;

	protected override bool CanSend3(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Special_Einstein_FallOn:
				return IsMine;
		}
		return false;
	}

	protected override void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Special_Einstein_FallOn:
				Vector2 target = (Vector2)pParams[0];
				bomb.FallOn(target);
				break;
			default:
				OnMsgUnhandled(pReceivedMsg);
				break;
		}
	}

}
