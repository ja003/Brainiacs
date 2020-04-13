using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEinsteinPhoton : SpecialWeaponPhoton
{
	//[SerializeField] SpecialCurie truck;

	protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		base.HandleMsg(pReceivedMsg, pParams, bb);

		SpecialEinstein bomb = (SpecialEinstein)controller;
		switch(pReceivedMsg)
		{
			//case EPhotonMsg.Special_Curie_StartTruck:
			//	EDirection dir = (EDirection)pParams[0];
			//	Vector3 pos = (Vector3)pParams[1];
			//	truck.StartTruck(dir, pos);
			//	break;
			case EPhotonMsg.Special_Einstein_FallOn:
				Vector3 target = (Vector3)pParams[0];
				bomb.FallOn(target);
				break;
		}
	}

}
