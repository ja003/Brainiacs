﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller of Tesla clones (not actual clone!)
/// </summary>
public class SpecialTeslaClone : PlayerWeaponSpecialPrefab
{
	[SerializeField] Player playerPrefab = null;
	Player cloneInstance;

	protected override void OnInit() { }

	protected override void OnReturnToPool3() { }

	protected override void OnSetActive2(bool pValue) { }

	protected override void OnStopUse() { }

	const int CLONE_DURATION = 5;

	protected override void OnUse()
	{
		Debug.Log($"Tesla clone");

		Vector2 spawnPos = owner.WeaponController.GetProjectileStart().position;
		cloneInstance = InstanceFactory.Instantiate(playerPrefab.gameObject, spawnPos).GetComponent<Player>();
		PlayerInitInfo info = owner.InitInfo.Clone();
		info.PlayerType = EPlayerType.AI;
		cloneInstance.ai.IsTmp = true;
		cloneInstance.SetInfo(info, false);
		cloneInstance.Stats.OnRespawn();

		//stop ai brain for 1 sec
		cloneInstance.ai.StopBrain(1);
		//send clone in player's direction so they dont get each other in the way that much
		cloneInstance.ai.Movement.SetTarget(spawnPos + Utils.GetVector2(playerDirection) * 2);

		cloneInstance.SetActive(true);
		cloneInstance.ai.Owner = owner;

		//store reference and pass to Destroy function.
		//reason: another clone might have been instanced before this one is destroyed
		Player cloneRef = cloneInstance;

		//todo: ignore owner
		//cloneInstance.ai.;
		DoInTime(() => DestroyClone(cloneRef), CLONE_DURATION);
	}

	private void DestroyClone(Player pClone)
	{
		if(!pClone.gameObject.activeSelf)
			return;

		pClone.ReturnToPool();
	}
}
