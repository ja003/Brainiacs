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
	//[SerializeField] SpecialNobelMinePhoton photon = null;

	bool isExploded;

	protected override void OnUse()
	{
		Debug.Log(gameObject.name + " OnUse");
		//spriteRend.enabled = false; //this is just holder, anmator is in child
		SetActive(true);
		mineSprite.sortingOrder = owner.Visual.GetProjectileSortOrder();

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

		Player player = collision.GetComponent<Player>();

		if(player == null)
		{
			//Debug.Log("Not player");
			return;
		}
		if(player.Equals(owner))
		{
			//Debug.Log("thats me ");
			return;
		}

		int finalDamage = owner.InitInfo.Hero == EHero.Nobel ? damage * 2 : damage;
		handler.OnCollision(finalDamage, owner, gameObject);
		Explode();
	}

	private void Explode()
	{
		Debug.Log("Explode");
		isExploded = true;
		animator.SetBool("explode", true);
	}

	internal void OnExplosionStateEnter()
	{
		mineSprite.sortingOrder = SortLayerManager.GetSortIndex(ESortObject.MapObject);
	}

	internal void OnExplosionStateExit()
	{
		//Debug.Log("OnExplosionStateExit");
		if(Photon.IsMine)
			ReturnToPool();
	}

	protected override void OnReturnToPool3()
	{
		isExploded = false;
	}

}
