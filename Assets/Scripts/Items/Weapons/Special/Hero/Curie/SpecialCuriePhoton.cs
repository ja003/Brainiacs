using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCuriePhoton : SpecialWeaponPhoton
{
	//[SerializeField] SpecialCurie truck;

	protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		base.HandleMsg(pReceivedMsg, pParams, bb);

		SpecialCurie truck = (SpecialCurie)controller;
		switch(pReceivedMsg)
		{
			//case EPhotonMsg.Special_Curie_StartTruck:
			//	EDirection dir = (EDirection)pParams[0];
			//	Vector3 pos = (Vector3)pParams[1];
			//	truck.StartTruck(dir, pos);
			//	break;
			case EPhotonMsg.Special_Curie_Collide:
				truck.Collide();
				break;
		}
	}

}
