using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DaVinci tank
/// Activated while use button is pressed.
/// It block X hits from projectile then it is destroyed => triggers reload
/// </summary>
public class SpecialDaVinciTank : PlayerWeaponSpecialPrefab, ICollisionHandler
{
	[SerializeField] int damage;
	[SerializeField] int maxHealth;

	int currentHealth;
		
	protected override void OnInit()
	{
		//Debug.Log(gameObject.name + " OnInit");
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), owner.Movement.PlayerCollider);

		transform.parent = owner.WeaponController.transform;
		transform.localPosition = Vector2.zero;

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
		//Debug.Log("OnUse");
		UpdateSortOrder();
		//prevent weapon change
		owner.WeaponController.CanSwapWeapon = false;

		//todo: make player invulnerable (he can be hit by bomb etc..)

		SetActive(true);

	}

	protected override void OnStopUse()
	{
		//Debug.Log("OnStopUse");
		//allow weapon change
		owner.WeaponController.CanSwapWeapon = true;
		SetActive(false);
	}

	public override void OnStartReloadWeapon()
	{
		currentHealth = maxHealth;
	}

	Dictionary<ICollisionHandler, float> collisionTimes = new Dictionary<ICollisionHandler, float>();
	const float MIN_COLLISION_DELAY = 0.5f;

	//private void OnTriggerEnter2D(Collider2D collision)
	//private void OnCollisionEnter2D(Collision2D collision)
	private void OnCollisionStay2D(Collision2D collision)
	{
		//Debug.Log("OnCollisionEnter2D " + collision.gameObject.name);

		ICollisionHandler handler = collision.gameObject.GetComponent<ICollisionHandler>();
		if(handler == null)
			return;

		float lastCollisionTime;
		if(collisionTimes.TryGetValue(handler, out lastCollisionTime) && 
			lastCollisionTime > Time.time - MIN_COLLISION_DELAY)
		{
			Debug.Log("Collision too soon");
			return;
		}

		if(collision.gameObject == owner.gameObject)
		{
			//Debug.Log("thats me ");
			return;
		}

		SoundController.PlaySound(ESound.Davinci_Tank_Hit, audioSource);
		//Debug.Log("Hit " + collision.gameObject.name);
		handler.OnCollision(damage, owner, gameObject);

		if(collisionTimes.ContainsKey(handler))
			collisionTimes[handler] =  Time.time;
		else
			collisionTimes.Add(handler, Time.time);

	}

	public bool OnCollision(int pDamage, Player pOwner, GameObject pOrigin)
	{
		SoundController.PlaySound(ESound.Davinci_Tank_Hit, audioSource);

		if(!owner.IsInitedAndMe)
			return false;

		if(pDamage > 0)
		{
			currentHealth--;
			if(currentHealth <= 0)
			{
				//Debug.Log("Destroyed");
				owner.WeaponController.ActiveWeapon.AmmoLeft = 0;
				Debug.LogWarning("TODO: crash anim + sound");
			}

			//Debug.Log("remains: " + currentHealth);
			return true;
		}

		return false;
	}
}
