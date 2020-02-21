using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeaponSpecialController : GameBehaviour
{
	protected PlayerWeaponController weaponContoller;
	protected Player owner;

	public abstract void Use();
	public virtual void StopUse() { }

	protected EDirection currentDirection => owner.Movement.CurrentDirection;


	public void Init(Player pOwner)
	{
		owner = pOwner;
		weaponContoller = pOwner.WeaponController;
		if(GetCollider() != null)
			Physics2D.IgnoreCollision(GetCollider(), pOwner.Collider);
		//Debug.Log($"Ignore collisions between {boxCollider2D.gameObject.name} and {owner}");

		OnInit();
	}

	protected abstract void OnInit();

	protected virtual Collider2D GetCollider()
	{
		return boxCollider2D;
	}

	internal virtual void OnDirectionChange(EDirection pDirection)
	{
	}

	internal virtual void OnSetActive()
	{
	}
}
