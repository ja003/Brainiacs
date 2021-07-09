using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DaVinci tank
/// Activated while use button is pressed.
/// It block X hits from projectile then it is destroyed => triggers reload
/// </summary>
public class SpecialDaVinciTank : PlayerWeaponSpecialPrefab, ICollisionHandler, IOwner
{
	[SerializeField] int damage;
	[SerializeField] int maxHealth;

	int currentHealth;

	UIHealthbar healthbar;

	protected override void OnInit()
	{
		//Debug.Log(gameObject.name + " OnInit");
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), owner.Movement.PlayerCollider);

		transform.SetParent(owner.WeaponController.transform);
		transform.localPosition = Vector2.zero;

		owner.Visual.OnSortOrderChanged += UpdateSortOrder;
		currentHealth = maxHealth;

		healthbar = InstanceFactory.Instantiate("p_Healthbar", Vector2.zero, false).GetComponent<UIHealthbar>();
		healthbar.Init(this, Vector2.up, false, Color.blue);

		UpdateSortOrder(); //needs to be updated on image side

		owner.Health.MyTank = this;
	}

	protected override void OnReturnToPool3()
	{
	}

	protected override void OnSetActive2(bool pValue)
	{
		//Debug.Log("OnSetActive2 " + pValue);
		spriteRend.enabled = pValue;
		//needed in MP...right?
		//collider has to be active for projectiles to be blocked etc
		boxCollider2D.enabled = pValue;// && Photon.IsMine;

		if(!isInited)
			return;

		healthbar.SetVisibility(pValue);
		owner.Health.Healthbar?.SetVisibility(!pValue);

	}
	private void UpdateSortOrder()
	{
		//if(!spriteRend.enabled)
		//	return;
		spriteRend.sortingOrder = owner.Visual.GetPlayerOverlaySortOrder();

		//image doesnt have ActiveWeapon set
		if(!owner.IsItMe)
			return;

		if(owner.WeaponController.ActiveWeapon == null)
		{
			Debug.LogError("No active weapon set");
			return;
		}

		//bug: player dies in tank => tank is not reseted
		if(gameObject.activeSelf && owner.WeaponController.ActiveWeapon.Id != EWeaponId.Special_DaVinci)
		{
			Debug.LogError("DaVinci tank still active");
			StopUse();
		}

		//Debug.Log("sortingOrder = " + spriteRend.sortingOrder);
	}

	protected override void OnUse()
	{
		//Debug.Log("OnUse");
		UpdateSortOrder();
		//prevent weapon change
		owner.WeaponController.CanSwapWeapon = false;


		//todo: make player invulnerable (he can be hit by bomb etc..)
		//owner.Collider.enabled = false; //doesnt work in MP
		//...or he doesnt have to be invulnerable?

		SetActive(true);

	}

	protected override void OnStopUse()
	{
		owner.Collider.enabled = true;

		//Debug.Log("OnStopUse");
		//allow weapon change
		owner.WeaponController.CanSwapWeapon = true;
		SetActive(false);
	}

	private void SetHealth(int pHealth)
	{
		currentHealth = pHealth;
		UpdateHealtbar(pHealth);
		//Debug.Log("Tank health: " + currentHealth);

		if(currentHealth <= 0)
		{
			//Debug.Log("Destroyed");
			owner.WeaponController.ActiveWeapon.AmmoLeft = 0;
			Debug.LogWarning("TODO: crash anim + sound");
			if(debug.InfiniteAmmo)
				Debug.Log("DaVinci tank wont destroy when InfiniteAmmo is on");

		}
	}

	public void UpdateHealtbar(int pHealth)
	{
		healthbar.SetHealth(pHealth, maxHealth);
		Photon.Send(EPhotonMsg.Special_DaVinci_UpdateHealthbar, pHealth);
	}

	public override void OnStartReloadWeapon()
	{
		SetHealth(maxHealth);
	}

	Dictionary<ICollisionHandler, float> collisionTimes = new Dictionary<ICollisionHandler, float>();
	const float MIN_COLLISION_DELAY = 0.5f;

	//private void OnTriggerEnter2D(Collider2D collision)
	//private void OnCollisionEnter2D(Collision2D collision)
	private void OnCollisionStay2D(Collision2D collision)
	{
		//collisions are handled only on owner side
		//collider has to be active for projectiles to be blocked etc
		if(!owner.IsItMe)
		{
			//Debug.Log("thats me ");
			return;
		}

		//Debug.Log($"{gameObject.name} OnCollisionEnter2D {collision.gameObject.name}");

		ICollisionHandler handler = collision.gameObject.GetComponent<ICollisionHandler>();
		if(handler == null)
			return;

		float lastCollisionTime;
		if(collisionTimes.TryGetValue(handler, out lastCollisionTime) &&
			lastCollisionTime > Time.time - MIN_COLLISION_DELAY)
		{
			//Debug.Log("Collision too soon");
			return;
		}

		if(collision.gameObject == owner.gameObject)
		{
			//Debug.Log("thats me ");
			return;
		}

		brainiacs.AudioManager.PlaySound(ESound.Davinci_Tank_Hit, audioSource);
		Vector2 push = GetPush(collision.transform);
		//Debug.Log($"Hit {collision.gameObject.name}. {push}");
		handler.OnCollision(damage, owner, gameObject, push);

		if(collisionTimes.ContainsKey(handler))
			collisionTimes[handler] = Time.time;
		else
			collisionTimes.Add(handler, Time.time);

	}

	public bool OnCollision(int pDamage, Player pOwner, GameObject pOrigin, Vector2 pPush)
	{
		brainiacs.AudioManager.PlaySound(ESound.Davinci_Tank_Hit, audioSource);

		if(!owner.IsInited)
			return false;

		if(!owner.IsItMe)
		{
			//we dont care about pOwner and pOrigin
			Photon.Send(EPhotonMsg.Special_DaVinci_OnCollision, pDamage, pPush);
			return pDamage > 0;
		}

		//Debug.Log("Tank OnCollision: " + pDamage);

		owner.Push.Push(pPush);

		if(pDamage > 0)
		{
			SetHealth(currentHealth - 1);
			return true;
		}

		return false;
	}

	public Player GetOwner()
	{
		return owner;
	}
}
