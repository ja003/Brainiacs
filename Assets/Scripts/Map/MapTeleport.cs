using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTeleport : MapObstackle
{
	protected override void OnCollisionEffect(int pDamage)
	{
		base.OnCollisionEffect(pDamage);
		//Debug.Log("todo: teleport projectile");
	}
}
