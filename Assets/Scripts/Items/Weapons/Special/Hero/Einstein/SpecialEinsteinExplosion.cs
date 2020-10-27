using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEinsteinExplosion : BrainiacsBehaviour
{
	//SpecialEinstein controller;
	int maxDamage;
	Player owner;

	public void OnInit(Player pOwner, int pMaxDamage)
	{
		spriteRend.sortingOrder = 0; //reset
		owner = pOwner;
		maxDamage = pMaxDamage;
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

		if(collision.gameObject == owner.gameObject)
		{
			//Debug.Log("thats me ");
			return;
		}

		float dist = Vector2.Distance(transform.position, collision.transform.position);
		float factor = circleCollider2D.radius - dist;

		float damage = Mathf.Lerp(maxDamage / 10f, maxDamage, factor);

		//Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);
		handler.OnCollision((int)damage, owner, gameObject);

		SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
		if(sr)
		{
			int orderAbove = sr.sortingOrder + 5;
			//multiple players can be hit - use the topmost order
			if(spriteRend.sortingOrder < orderAbove)
			{
				Debug.Log($"Set order {orderAbove}");
				spriteRend.sortingOrder = orderAbove;
			}
		}
	}


}
