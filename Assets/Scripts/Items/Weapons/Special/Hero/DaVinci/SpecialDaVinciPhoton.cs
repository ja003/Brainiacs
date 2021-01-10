using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialDaVinciPhoton : PlayerWeaponSpecialPrefabPhoton
{
	protected override bool CanSend3(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Special_DaVinci_UpdateHealthbar:
				return IsMine;

			case EPhotonMsg.Special_DaVinci_OnCollision:
				return !IsMine; //only image sends this info to master
		}

		return false;
	}
	[SerializeField] SpecialDaVinciTank tank = null;

	protected override void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Special_DaVinci_OnCollision:
				if(!view.IsMine)
					return;

				int damage = (int)pParams[0];
				Vector2 push = (Vector2)pParams[1];
				tank.OnCollision(damage, null, null, push);
				break;
			case EPhotonMsg.Special_DaVinci_UpdateHealthbar:				
				int health = (int)pParams[0];
				tank.UpdateHealtbar(health);
				break;
			default:
				OnMsgUnhandled(pReceivedMsg); //call SpecialWeaponPhoton instead
				break;
		}
	}
}
