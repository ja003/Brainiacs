using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerController : PlayerWeaponSpecialController, IOnCollision
{
	[SerializeField]
	private PolygonCollider2D flameCollider;

	[SerializeField]
	private new Animator animator;

	[SerializeField]
	private CollisionDetector collisionDetector;

	private void Update()
	{
		//BUG: fire key-up is sometimes not registered
		//TODO: maybe put to input controller
		if(isUsed && Time.time > lastTimeUsed + 0.2f)
		{
			Debug.Log($"Flamethrower Auto - STOP USE");
			StopUse();
		}
	}

	float lastTimeUsed;
	bool isUsed;
	private void SetUse(bool pValue)
	{
		isUsed = pValue;
		animator.SetBool("isUsed", isUsed);
		flameCollider.enabled = isUsed;
	}

	public override void Use()
	{
		SetUse(true);
		Debug.Log($"Flamethrower USE");
		lastTimeUsed = Time.time;
	}

	public override void StopUse()
	{
		SetUse(false);
		Debug.Log($"Flamethrower STOP-USE");
	}

	protected override void OnInit()
	{
		StopUse();
		collisionDetector.Init(this);
	}

	protected override Collider2D GetCollider()
	{
		return flameCollider;
	}

	public void OnTriggerStay2D(Collider2D pCollision)
	{
		if(!isUsed)
			return;
		//Debug.Log($"Flamethrower OnTriggerStay2D " + pCollision.gameObject.name);
	}
}
