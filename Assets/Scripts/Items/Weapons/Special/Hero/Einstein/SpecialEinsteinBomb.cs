using FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
		Vector2 target = GetTargetPosition();
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


	public void FallOn(Vector2 pTarget)
	{
		transform.position = pTarget + Vector2.up * 10;
		LeanTween.moveY(gameObject, pTarget.y, 2).setOnComplete(Explode);
		Photon.Send(EPhotonMsg.Special_Einstein_FallOn, pTarget);

	}

	private void Explode()
	{
		//Debug.Log("BOOOM");
		spriteRend.enabled = false;
		explosion.Explode();
		PlaySound(ESound.Einstein_Explosion);
	}

	internal void OnExplosionStateExit()
	{
		//transform.parent = weaponContoller.transform;
		//explosion.SetEnabled(false);
		if(Photon.IsMine)
			ReturnToPool();
	}

	private Vector2 GetTargetPosition()
	{
		var otherPlayers = game.PlayerManager.GetOtherPlayers(owner, true);
		Vector2 targetedPlayerPos;

		if(otherPlayers.Count == 0)
			targetedPlayerPos = owner.transform.position;
		else
		{
			int randomPlayerIndex = Random.Range(0, otherPlayers.Count);
			targetedPlayerPos = otherPlayers[randomPlayerIndex].transform.position;
		}
		const float target_offset = 1f;
		Vector2 randomOffset = new Vector2(
				Random.Range(-target_offset, target_offset),
				Random.Range(-target_offset, target_offset));
		Vector2 finalTargetPos = targetedPlayerPos + randomOffset;

		Utils.DebugDrawCross(finalTargetPos, Color.cyan, 1);
		return finalTargetPos;
	}
}
