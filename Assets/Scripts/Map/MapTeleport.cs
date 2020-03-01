using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTeleport : MapObstackle
{
	protected override void OnCollisionEffect(Projectile pProjectile)
	{
		base.OnCollisionEffect(pProjectile);
		Debug.Log("todo: teleport projectile");
	}
}
