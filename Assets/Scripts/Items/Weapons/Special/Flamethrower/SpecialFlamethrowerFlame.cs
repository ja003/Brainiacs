using FlatBuffers;
using System;
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

	private void Update()
	{
		const float auto_stop_use_delay = 0.1f;
		if(isUsed && Time.time - lastTimeUsed > auto_stop_use_delay)
		{
			Debug.LogError("Flame auto-stopped");
			StopUse();
		}
	}

	protected override void OnReturnToPool3()
	{

	}


	protected override void OnInit()
	{
		SetActive(false);
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

		if(pValue)
		{
			//SetActive is called every frame => start audio loop only once
			if(!audioSource.isPlaying)
				brainiacs.AudioManager.PlayWeaponUseSound(EWeaponId.Flamethrower, audioSource, true);
		}
		else
		{
			audioSource.Stop();
		}
	}

	float lastTimeUsed;
	protected override void OnUse()
	{
		SetUse(true);
	}
	protected override void OnStopUse()
	{
		SetUse(false);
	}

	bool isUsed;
	private void SetUse(bool pValue)
	{
		isUsed = pValue;
		if(pValue)
			SetActive(pValue);

		if(animator.enabled)
			animator.SetBool("isUsed", pValue);
		collisionDetector.SetEnabled(pValue);

		if(pValue)
			lastTimeUsed = Time.time;
	}


	internal void OnFlameFinished()
	{
		SetActive(false);
	}

	//limit cadency of applied damage
	//copied from SpecialDaVinciTank - todo: refactor to common system
	Dictionary<Player, float> collisionTimes = new Dictionary<Player, float>();
	const float MIN_COLLISION_DELAY = 0.05f;

	/// <summary>
	/// Called from collision detector.
	/// Note: player rigidbody sleep mode has to be set to: Never sleep.
	/// Otherwise collision is sometimes ignored when other player is idle.
	/// </summary>
	public void Event_OnTriggerStay2D(Collider2D pCollision)
	{
		if(!owner.IsItMe)
		{
			Debug.LogError("Flame should collide only on player side");
			return;
		}

		Player player = pCollision.gameObject.GetComponent<Player>();
		if(player == null)
			return;

		float lastCollisionTime;
		if(collisionTimes.TryGetValue(player, out lastCollisionTime) &&
			lastCollisionTime > Time.time - MIN_COLLISION_DELAY)
		{
			//Debug.Log("Collision too soon");
			return;
		}

		if(collisionTimes.ContainsKey(player))
			collisionTimes[player] = Time.time;
		else
			collisionTimes.Add(player, Time.time);

		int damage = GetDamage(player.ColliderCenter);
		//Debug.Log($"Flamethrower  " + damage);
		player.Health.ApplyDamage(damage, owner);
	}

	private int GetDamage(Vector2 pTargetPosition)
	{
		float distance = Vector2.Distance(transform.position, pTargetPosition);
		float maxDist = Vector2.Distance(transform.position, maxDistance.position);
		float percentage = 1 - distance / maxDist;
		float damage = Mathf.Lerp(minDamage, maxDamage, percentage);

		//Debug.Log($"distance = {distance}, {percentage}% => {damage} damage");
		return (int)damage;
	}

	public void OnDirectionChange(EDirection pDirection)
	{
		//Debug.Log("OnDirectionChange " + pDirection);
		if(!isInited)
			return;

		transform.SetParent(owner.WeaponController.GetProjectileStart(pDirection));

		transform.localPosition = Vector2.zero;
		transform.localRotation = Quaternion.Euler(0, 0, 0);

		Vector3 rot = Utils.GetRotation(pDirection, 90);
		//Debug.Log("rot = " + rot);

		transform.Rotate(rot);

		Photon.Send(EPhotonMsg.Special_Flamethrower_OnDirectionChange, pDirection);
	}


}
