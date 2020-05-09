using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DaVinci tank
/// Activated while use button is pressed.
/// It block X hits from projectile then it is destroyed => triggers reload
/// </summary>
public class SpecialDaVinciTank : PlayerWeaponSpecialPrefab
{
	[SerializeField] int damage = 30;
	[SerializeField] int maxHealth = 3;

	int currentHealth;
		
	protected override void OnInit()
	{
		//Debug.Log(gameObject.name + " OnInit");
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), owner.Movement.PlayerCollider);

		transform.parent = owner.WeaponController.transform;
		transform.localPosition = Vector3.zero;

		owner.Visual.OnSortOrderChanged += UpdateSortOrder;
		currentHealth = maxHealth;
	}

	protected override void OnReturnToPool3()
	{
	}

	protected override void OnSetActive2(bool pValue)
	{
		SetEnabled(pValue);
	}

	private void SetEnabled(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue && Photon.IsMine;
	}

	private void UpdateSortOrder()
	{
		//if(!spriteRend.enabled)
		//	return;
		spriteRend.sortingOrder = owner.Visual.GetPlayerOverlaySortOrder();
		//Debug.Log("sortingOrder = " + spriteRend.sortingOrder);
	}

	protected override void OnUse()
	{
		UpdateSortOrder();
		//prevent weapon change
		owner.WeaponController.CanSwapWeapon = false;

		//todo: make player invulnerable (he can be hit by bomb etc..)

		SetActive(true);

	}

	protected override void OnStopUse()
	{
		//allow weapon change
		owner.WeaponController.CanSwapWeapon = true;
		SetActive(false);
	}

	public override void OnStartReloadWeapon()
	{
		currentHealth = maxHealth;
	}

	//private void OnTriggerEnter2D(Collider2D collision)
	private void OnCollisionEnter2D(Collision2D collision)
	{
		//Debug.Log("OnCollisionEnter2D " + collision.gameObject.name);

		ICollisionHandler handler = collision.gameObject.GetComponent<ICollisionHandler>();
		if(handler == null)
			return;

		if(collision.gameObject == owner.gameObject)
		{
			//Debug.Log("thats me ");
			return;
		}

		//Debug.Log("Hit " + collision.gameObject.name);
		handler.OnCollision(damage, owner);
	}

	public bool OnCollision(int pDamage, Player pOrigin)
	{
		if(!owner.IsInitedAndMe)
			return false;

		if(pDamage > 0)
		{
			currentHealth--;
			if(currentHealth <= 0)
			{
				//Debug.Log("Destroyed");
				owner.WeaponController.ActiveWeapon.AmmoLeft = 0;
			}

			//Debug.Log("remains: " + currentHealth);
			return true;
		}

		return false;
	}

}
