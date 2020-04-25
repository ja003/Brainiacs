using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerController : PlayerWeaponSpecialController, IOnCollision
{
	//[SerializeField] private PolygonCollider2D flameCollider = null;
	[SerializeField] private new Animator animator = null;
	[SerializeField] private CollisionDetector collisionDetector = null;
	[SerializeField] private Transform maxDistance = null;

	[SerializeField] private float minDamage = -1;
	[SerializeField] private float maxDamage = -1;

	private void Update()
	{
		//BUG: fire key-up is sometimes not registered
		//TODO: maybe put to input controller
		if(isUsed && Time.time > lastTimeUsed + 0.2f)
		{
			//Debug.Log($"Flamethrower Auto - STOP USE");
			StopUse();
		}
	}

	protected override void OnInit()
	{
		StopUse();
		collisionDetector.Init(this);
		gameObject.SetActive(true);
	}

	float lastTimeUsed;
	bool isUsed;
	private void SetUse(bool pValue)
	{
		isUsed = pValue;
		animator.SetBool("isUsed", isUsed);
		collisionDetector.Collider2D.enabled = isUsed;

		if(pValue)
			lastTimeUsed = Time.time;
	}

	protected override void OnUse()
	{
		SetUse(true);
		//Debug.Log($"Flamethrower USE");
	}

	protected override void OnStopUse()
	{
		SetUse(false);
		//Debug.Log($"Flamethrower STOP-USE");
	}


	protected override Collider2D GetCollider()
	{
		return collisionDetector.Collider2D;
	}

	/// <summary>
	/// Called from collision detector.
	/// Note: player rigidbody sleepmode has to be set to: Never sleep.
	/// Otherwise collision is sometimes ignored when other player is idle.
	/// </summary>
	public void Event_OnTriggerStay2D(Collider2D pCollision)
	{
		if(!isUsed)
			return;
		Player player = pCollision.gameObject.GetComponent<Player>();
		if(player)
		{
			int damage = GetDamage(player.transform.position);
			//Debug.Log($"Flamethrower  " + damage);
			player.Health.ApplyDamage(damage, Owner);
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

	internal override void OnSetActive()
	{
		base.OnSetActive();

		OnDirectionChange(playerDirection);
	}

	internal override void OnDirectionChange(EDirection pDirection)
	{
		if(!weaponContoller)
		{
			if(isInited)
				Debug.LogError("weaponContoller not assigned");
			return;
		}

		base.OnDirectionChange(pDirection);

		transform.parent = weaponContoller.GetProjectileStart(pDirection);

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0, 0, 0);

		Vector3 rot = Utils.GetRotation(pDirection, 90);
		transform.Rotate(rot);

		Photon.Send(EPhotonMsg.Special_Flamethrower_OnDirectionChange, pDirection);
	}
}
