using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCurieTruckPhoton : PlayerWeaponSpecialPrefabPhoton
{
	protected override bool CanSend3(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Special_Curie_StartTruck:
			case EPhotonMsg.Special_Curie_Collide:
				return IsMine;
		}

		return false;
	}

	[SerializeField] SpecialCurieTruck truck = null;

	protected override void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		//SpecialCurieTruck truck = GetComponent<SpecialCurieTruck>();
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Special_Curie_StartTruck:
				EDirection dir = (EDirection)pParams[0];
				Vector3 pos = (Vector3)pParams[1];
				truck.StartTruck(dir, pos);
				break;
			case EPhotonMsg.Special_Curie_Collide:
				truck.Collide();
				break;
			default:
				OnMsgUnhandled(pReceivedMsg); //call SpecialWeaponPhoton instead
				break;
		}
		
	}

}
