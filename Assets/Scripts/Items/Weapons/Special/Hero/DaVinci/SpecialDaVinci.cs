using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DaVinci tank
/// Activated while use button is pressed.
/// It block X hits from projectile then it is destroyed => triggers reload
/// </summary>
public class SpecialDaVinci : PlayerWeaponSpecialController, ICollisionHandler
{
	[SerializeField] int damage = 30;
	[SerializeField] int maxHealth = 3;

	int currentHealth;

	protected override void OnInit()
	{
		SetEnabled(false);
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Owner.Movement.PlayerCollider);

		transform.parent = weaponContoller.transform;
		transform.localPosition = Vector3.zero;

		Owner.Visual.OnSortOrderChanged += UpdateSortOrder;

		currentHealth = maxHealth;
	}

	private void UpdateSortOrder()
	{
		if(!spriteRend.enabled)
			return;
		spriteRend.sortingOrder = Owner.Visual.GetPlayerOverlaySortOrder();
		//Debug.Log("sortingOrder = " + spriteRend.sortingOrder);
	}

	private void SetEnabled(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue;
	}


	protected override void OnUse()
	{
		UpdateSortOrder();
		//prevent weapon change
		weaponContoller.CanSwapWeapon = false;

		//todo: make player invulnerable (he can be hit by bomb etc..)

		SetEnabled(true);

	}

	protected override void OnStopUse()
	{
		weaponContoller.CanSwapWeapon = true;
		SetEnabled(false);
		//allow weapon change
	}

	internal override void OnStartReloadWeapon()
	{
		base.OnStartReloadWeapon();
		currentHealth = maxHealth;
	}

	//private void OnTriggerEnter2D(Collider2D collision)
	private void OnCollisionEnter2D(Collision2D collision)
	{
		//Debug.Log("OnCollisionEnter2D " + collision.gameObject.name);

		ICollisionHandler handler = collision.gameObject.GetComponent<ICollisionHandler>();
		if(handler == null)
			return;

		if(collision.gameObject == Owner.gameObject)
		{
			//Debug.Log("thats me ");
			return;
		}

		Debug.Log("Hit " + collision.gameObject.name);
		handler.OnCollision(damage, Owner);
	}

	public bool OnCollision(int pDamage, Player pOrigin)
	{
		if(!Owner.IsInitedAndMe)
			return false;

		if(pDamage > 0)
		{
			currentHealth--;
			if(currentHealth <= 0)
			{
				//Debug.Log("Destroyed");
				weaponContoller.ActiveWeapon.AmmoLeft = 0;
			}

			//Debug.Log("remains: " + currentHealth);
			return true;
		}

		return false;
	}
}
