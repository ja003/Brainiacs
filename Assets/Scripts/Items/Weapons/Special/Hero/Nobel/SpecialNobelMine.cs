using FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prefab shared by nobel special weapon and map weapon mine.
/// Explosion damage is extra if the owner is Nobel.
/// </summary>
public class SpecialNobelMine : PlayerWeaponSpecialPrefab
{
	[SerializeField] int damage; //base damage. if owner is Nobel => 2 * damage

	//[SerializeField] Animator mineAnimator = null;
	[SerializeField] SpriteRenderer mineSprite = null;
	[SerializeField] SpriteRenderer explosionSprite = null;
	//[SerializeField] SpecialNobelMinePhoton photon = null;

	bool isExploded;

	protected override void OnUse()
	{
		//Debug.Log(gameObject.name + " OnUse");
		//spriteRend.enabled = false; //this is just holder, anmator is in child
		SetActive(true);
		mineSprite.sortingOrder =
			//owner.Visual.GetProjectileSortOrder();
			owner.Visual.CurrentSortOrder - 1;


		transform.position = owner.WeaponController.GetProjectileStart().position;
		//Photon.Send(EPhotonMsg.Special_Nobel_Spawn, owner.InitInfo.Number);		
		isExploded = false;
	}

	protected override void OnStopUse()
	{
	}

	protected override void OnInit()
	{
	}

	protected override void OnSetActive2(bool pValue)
	{
		mineSprite.enabled = pValue;
		animator.enabled = pValue;
		//mine is simulated only on its owner side
		boxCollider2D.enabled = pValue && owner.IsItMe;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(!owner.IsItMe)
		{
			Debug.LogError("Mine shouldnt be triggered on its owner image side");
			return;
		}

		if(isExploded || !isInited)
			return;

		ICollisionHandler handler = collision.GetComponent<ICollisionHandler>();
		if(handler == null)
			return;

		IOwner iOwner = collision.GetComponent<IOwner>();
		Player triggerOrigin = iOwner?.GetOwner();

		//we should hit even non-player (eg. DaVinci tank)
		//if(player == null)
		//{
		//	Debug.Log("Not player");
		//	return;
		//}
		if(triggerOrigin != null && triggerOrigin.Equals(owner))
		{
			//Debug.Log("thats me ");
			return;
		}

		int finalDamage = owner.InitInfo.Hero == EHero.Nobel ? damage * 2 : damage;
		Transform transform = triggerOrigin != null ? triggerOrigin.transform : collision.GetComponent<Transform>();
		handler.OnCollision(finalDamage, owner, gameObject, GetPush(transform));

		//set sort order above the player who triggered the explosion
		int explosionSortOrder = triggerOrigin.Visual.GetProjectileSortOrder();
		Explode(explosionSortOrder, false);
	}

	public void Explode(int pSortOrder, bool pIsRPC)
	{
		//Debug.Log("Explode");
		isExploded = true;
		animator.SetBool("explode", true);
		brainiacs.AudioManager.PlaySound(ESound.Nobel_Mine_Explode, audioSource);

		mineSprite.enabled = false;
		explosionSprite.enabled = true;
		explosionSprite.sortingOrder = pSortOrder;

		if(!pIsRPC)
			Photon.Send(EPhotonMsg.Special_Nobel_Explode, pSortOrder);
	}

	internal void OnExplosionStateEnter()
	{
	}

	internal void OnExplosionStateExit()
	{
		//the explosion animation seems to repeat on remote image => disable it
		explosionSprite.enabled = false;
		if(Photon.IsMine)
			ReturnToPool();
	}

	protected override void OnReturnToPool3()
	{
		isExploded = false;
	}

}
