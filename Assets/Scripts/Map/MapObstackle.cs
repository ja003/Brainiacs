﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstackle : MapObject
{
	protected override void OnSetActive0(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue;
	}


	//protected override bool CanSendMsg(EPhotonMsg pMsgType)
	//{
	//	throw new System.NotImplementedException();
	//}

	protected override void OnCollisionEffect(int pDamage, GameObject pOrigin)
	{
		//Debug.Log("todo: play collision sound, make effect...");
		if(pDamage > 0)
		{
			SoundController.PlaySound(ESound.Map_Obstackle_Hit, audioSource);
		}
	}

	
}
