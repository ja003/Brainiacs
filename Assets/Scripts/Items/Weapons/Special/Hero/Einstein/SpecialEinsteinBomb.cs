using FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEinsteinBomb : PlayerWeaponSpecialPrefab
{
	[SerializeField] SpecialEinsteinExplosion explosion = null;
	[SerializeField] public int MaxDamage = 100;

	protected override void OnInit()
	{
		//SetActive(true);
		spriteRend.enabled = false;
		explosion.OnInit(owner, MaxDamage);
	}

	protected override void OnUse()
	{
		SetActive(true);
		Vector3 target = GetTargetPosition();
		if(!owner.IsLocalImage)
		{
			FallOn(target);
		}
		//explosion.OnSpawn(owner, MaxDamage);
	}

	protected override void OnStopUse()
	{
	}

	protected override void OnSetActive2(bool pValue)
	{
		spriteRend.enabled = pValue;
		//explosion.OnSetActive(pValue);
	}

	protected override void OnReturnToPool3()
	{
		explosion.SetEnabled(false);
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
		explosion.Explode();
	}

	internal void OnExplosionStateExit()
	{
		//transform.parent = weaponContoller.transform;
		//explosion.SetEnabled(false);
		if(Photon.IsMine)
			ReturnToPool();
	}

	private Vector3 GetTargetPosition()
	{
		return Vector3.zero;
	}
}
