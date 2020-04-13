using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEinstein : PlayerWeaponSpecialController
{
	[SerializeField] SpecialEinsteinExplosion explosion = null;
	[SerializeField] public int MaxDamage = 100;

	protected override void OnInit()
	{
		gameObject.SetActive(true);
		spriteRend.enabled = false;
		explosion.Init(this);
	}

	protected override void OnUse()
	{
		explosion.SetEnabled(false);
		spriteRend.enabled = true;
		transform.parent = game.ProjectileManager.transform;

		Vector3 target = GetTargetPosition();
		if(!Owner.IsLocalImage)
		{
			FallOn(target);
		}
	}
	protected override void OnStopUse()
	{
	}

	public void FallOn(Vector3 pTarget)
	{
		transform.position = pTarget + Vector3.up * 10;
		LeanTween.moveY(gameObject, pTarget.y, 2).setOnComplete(Explode);
		Photon.Send(EPhotonMsg.Special_Einstein_FallOn, pTarget);

	}

	private void Explode()
	{
		//Debug.Log("BOOOM");
		spriteRend.enabled = false;
		explosion.SetEnabled(true);
	}

	internal void OnExplosionStateExit()
	{
		transform.parent = weaponContoller.transform;
		explosion.SetEnabled(false);
	}

	private Vector3 GetTargetPosition()
	{
		return Vector3.zero;
	}

	

}
