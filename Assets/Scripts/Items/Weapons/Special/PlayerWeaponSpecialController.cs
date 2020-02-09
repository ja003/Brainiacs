using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeaponSpecialController : GameBehaviour
{
	protected PlayerWeaponController weaponContoller;
	protected Player owner;

	public abstract void Use();

	public void Init(Player pOwner)
	{
		owner = pOwner;
		weaponContoller = pOwner.WeaponController;
		Physics2D.IgnoreCollision(boxCollider2D, pOwner.Collider);
		//Debug.Log($"Ignore collisions between {boxCollider2D.gameObject.name} and {owner}");

		OnInit();
	}

	protected abstract void OnInit();
}
