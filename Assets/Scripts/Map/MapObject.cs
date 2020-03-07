using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : BrainiacsBehaviour, IProjectileCollisionHandler
{
	public bool OnCollision(Projectile pProjectile)
	{
		OnCollisionEffect(pProjectile);
		return true;
	}

	protected abstract void OnCollisionEffect(Projectile pProjectile);
}
