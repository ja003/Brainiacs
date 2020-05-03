using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstackle : MapObject
{
	protected override void OnSetActive(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue;
	}


	//protected override bool CanSendMsg(EPhotonMsg pMsgType)
	//{
	//	throw new System.NotImplementedException();
	//}

	protected override void OnCollisionEffect(int pDamage)
	{
		//Debug.Log("todo: play collision sound, make effect...");
	}
}
