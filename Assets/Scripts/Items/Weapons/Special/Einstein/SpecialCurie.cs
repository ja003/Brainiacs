using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCurie : PlayerWeaponSpecialController
{

	public override void Use()
	{
		gameObject.SetActive(true);
		transform.parent = game.ProjectileManager.transform;

		transform.position = weaponContoller.transform.position;
		Vector3 dir = Utils.GetVector3(currentDirection);

		//BUG?
		//it seems that IgnoreCollision is reset after deactivation
		Physics2D.IgnoreCollision(boxCollider2D, owner.Collider);

		LeanTween.cancel(gameObject);
		LeanTween.move(gameObject, transform.position + dir * 100, 10);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log("BOOM");
		gameObject.SetActive(false);
	}


	protected override void OnInit()
	{
		gameObject.SetActive(false);

	}

}
