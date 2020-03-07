using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEinstein : PlayerWeaponSpecialController
{

	public override void Use()
	{
		gameObject.SetActive(true);

		transform.parent = game.ProjectileManager.transform;

		Vector3 target = GetTargetPosition();
		transform.position = target + Vector3.up * 10;

		LeanTween.moveY(gameObject, target.y, 2).setOnComplete(OnImpact);
	}

	private void OnImpact()
	{
		Debug.Log("BOOOM");
		gameObject.SetActive(false);
		transform.parent = weaponContoller.transform;
	}

	private Vector3 GetTargetPosition()
	{
		return Vector3.zero;
	}

	protected override void OnInit()
	{
		gameObject.SetActive(false);

	}

}
