using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : PoolObject, ICollisionHandler
{
	public bool OnCollision(int pDamage, Player pOrigin)
	{
		OnCollisionEffect(pDamage);
		//if(pDamage > 0)
		//	gameObject.SetActive(false);
		//todo: implement powerup class -> might explode on collision?

		return true;
	}

	protected abstract void OnCollisionEffect(int pDamage);
}
