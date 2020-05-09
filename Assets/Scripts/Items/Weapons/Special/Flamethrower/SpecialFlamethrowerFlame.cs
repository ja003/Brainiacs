﻿using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFlamethrowerFlame : PlayerWeaponSpecialPrefab, IOnCollision
{
	//[SerializeField] private PolygonCollider2D flameCollider = null;
	//[SerializeField] private new Animator animator = null;
	[SerializeField] private new SpriteRenderer spriteRend = null;
	[SerializeField] private CollisionDetector collisionDetector = null;
	[SerializeField] private Transform maxDistance = null;

	[SerializeField] private float minDamage = -1;
	[SerializeField] private float maxDamage = -1;

	//Player owner;
	//public void Init(Player pOwner)
	//{
	//	owner = pOwner;
	//	SetUse(false);

	//	if(owner.IsItMe) //flame collision detected on player side
	//		collisionDetector.Init(this);

	//	OnDirectionChange(EDirection.Right); //initial refresh
	//	//SetActive(true);
	//}

	protected override void OnReturnToPool3()
	{

	}

	protected override void OnInit()
	{
		StopUse();

		if(owner.IsItMe) //flame collision detected on player side
			collisionDetector.Init(this);

		OnDirectionChange(EDirection.Right); //initial refresh
	}

	protected override void OnSetActive2(bool pValue)
	{
		animator.enabled = pValue;
		spriteRend.enabled = pValue;
		collisionDetector.SetEnabled(pValue && owner.IsItMe);
	}

	protected override void OnUse()
	{
		SetUse(true);
	}
	protected override void OnStopUse()
	{
		SetUse(false);
	}

	private void SetUse(bool pValue)
	{
		//isUsed = pValue;
		SetActive(true);
		animator.SetBool("isUsed", pValue);
		collisionDetector.SetEnabled(pValue);

		//if(pValue)
		//	lastTimeUsed = Time.time;
	}


	/// <summary>
	/// Called from collision detector.
	/// Note: player rigidbody sleepmode has to be set to: Never sleep.
	/// Otherwise collision is sometimes ignored when other player is idle.
	/// </summary>
	public void Event_OnTriggerStay2D(Collider2D pCollision)
	{
		if(!owner.IsItMe)
		{
			Debug.LogError("Flame should collide only on player side");
			return;
		}
		//if(!isUsed)
		//	return;

		Player player = pCollision.gameObject.GetComponent<Player>();
		if(player)
		{
			int damage = GetDamage(player.transform.position);
			//Debug.Log($"Flamethrower  " + damage);
			player.Health.ApplyDamage(damage, owner);
		}
	}

	private int GetDamage(Vector3 pTargetPosition)
	{
		float distance = Vector3.Distance(transform.position, pTargetPosition);
		float maxDist = Vector3.Distance(transform.position, maxDistance.position);
		float percentage = 1 - distance / maxDist;
		float damage = Mathf.Lerp(minDamage, maxDamage, percentage);

		//Debug.Log($"distance = {distance}, {percentage}% => {damage} damage");
		return (int)damage;
	}

	

	public void OnDirectionChange(EDirection pDirection)
	{
		if(!isInited)
			return;

		transform.parent =  owner.WeaponController.GetProjectileStart(pDirection);

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0, 0, 0);

		Vector3 rot = Utils.GetRotation(pDirection, 90);
		transform.Rotate(rot);

		Photon.Send(EPhotonMsg.Special_Flamethrower_OnDirectionChange, pDirection);
	}


}