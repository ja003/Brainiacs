using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : PoolObjectNetwork, ICollisionHandler
{
	public bool OnCollision(int pDamage, Player pOwner, GameObject pOrigin, Vector2 pPush)
	{
		OnCollisionEffect(pDamage, pOrigin);
		//if(pDamage > 0)
		//	gameObject.SetActive(false);
		//todo: implement powerup class -> might explode on collision?

		bool? result2 = OnCollision2(pDamage, pOwner, pOrigin);
		if(result2 != null)
			return (bool)result2;

		return true;
	}

	/// <summary>
	/// Possible override of collision outcome
	/// </summary>
	protected virtual bool? OnCollision2(int pDamage, Player pOwner, GameObject pOrigin)
	{
		return null;
	}

	protected override void OnSetActive0(bool pValue)
	{
	}

	protected abstract void OnCollisionEffect(int pDamage, GameObject pOrigin);

	protected override void OnReturnToPool2()
	{
		//throw new NotImplementedException();
	}

}
