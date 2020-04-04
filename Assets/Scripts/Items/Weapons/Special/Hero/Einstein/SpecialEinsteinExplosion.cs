using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEinsteinExplosion : BrainiacsBehaviour
{
	SpecialEinstein controller;
	int maxDamage;

	public void Init(SpecialEinstein pController)
	{
		controller = pController;
		maxDamage = pController.MaxDamage;
		SetEnabled(false);
	}

	public void SetEnabled(bool pValue)
	{
		animator.enabled = pValue;
		if(pValue)
			animator.Rebind();
		spriteRend.enabled = pValue;

		circleCollider2D.enabled = pValue && controller.Owner.IsItMe;
	}

	//private void OnCollisionEnter2D(Collision2D collision)
	//{
	//	Debug.Log("OnCollisionEnter2D " + collision.gameObject.name);
	//}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		ICollisionHandler handler = collision.GetComponent<ICollisionHandler>();
		if(handler == null)
			return;

		if(collision.gameObject == controller.Owner.gameObject)
		{
			//Debug.Log("thats me ");
			return;
		}

		float dist = Vector3.Distance(transform.position, collision.transform.position);
		float factor = circleCollider2D.radius - dist;
		
		float damage = Mathf.Lerp(maxDamage / 10f, maxDamage, factor);

		//Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);
		handler.OnCollision((int)damage);
	}
}
