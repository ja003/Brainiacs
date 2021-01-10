using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEinsteinExplosion : BrainiacsBehaviour
{
	//SpecialEinstein controller;
	Player owner;
	SpecialEinsteinBomb bomb;

	public void OnInit(Player pOwner, SpecialEinsteinBomb pBomb)
	{
		spriteRend.sortingOrder = 0; //reset
		owner = pOwner;
		bomb = pBomb;
		SetEnabled(false);

		//owner.OnPlayerInited.AddAction(() => SetEnabled(false));
		//SetEnabled(false);
	}

	internal void Explode()
	{
		SetEnabled(true);
		animator.Rebind();
	}

	public void SetEnabled(bool pValue)
	{
		animator.enabled = pValue;
		spriteRend.enabled = pValue;
		circleCollider2D.enabled = pValue && owner.IsItMe;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		ICollisionHandler handler = collision.GetComponent<ICollisionHandler>();
		if(handler == null)
			return;

		float dist = Vector2.Distance(transform.position, collision.transform.position);
		float factor = circleCollider2D.radius - dist; //(0 - radius) //radius = 1,5

		//owner is pushed but not damaged
		bool isItOwner = collision.gameObject == owner.gameObject;
		float damage = isItOwner ? 0 : Mathf.Lerp(bomb.MaxDamage / 10f, bomb.MaxDamage, factor);

		Vector3 push = (collision.transform.position - transform.position).normalized
			* bomb.PushForce
			* (factor + 0.5f);
		//Debug.Log($"push force {factor} = {push.magnitude}");
		handler.OnCollision((int)damage, owner, gameObject,	push);

		SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
		if(sr)
		{
			int orderAbove = sr.sortingOrder + 5;
			//multiple players can be hit - use the topmost order
			if(spriteRend.sortingOrder < orderAbove)
			{
				//Debug.Log($"Set order {orderAbove}");
				spriteRend.sortingOrder = orderAbove;
			}
		}
	}


}
