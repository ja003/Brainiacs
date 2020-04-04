using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : BrainiacsBehaviour, ICollisionHandler
{
	public bool OnCollision(int pDamage)
	{
		OnCollisionEffect(pDamage);
		//if(pDamage > 0)
		//	gameObject.SetActive(false);
		//todo: implement powerup class -> might explode on collision?

		return true;
	}

	protected abstract void OnCollisionEffect(int pDamage);
}
