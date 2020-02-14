using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : GameBehaviour
{
	[SerializeField]
	private PlayerStats stats;

	[SerializeField]
	private PlayerVisual visual;

	[SerializeField]
	private PlayerMovement movement;
	
	public void OnHitBy(ProjectileConfig pConfig)
	{
		if(stats.IsDead())
			return;

		if(stats.DecreseHealth(pConfig.damage))
		{
			Die();
		}
	}

	private void Die()
	{
		Debug.Log($"{this} Die ({stats.LivesLeft} lives left)");

		visual.OnDie();

		if(stats.LivesLeft > 0)
		{
			DoInTime(Respawn, 2);
		}
	}

	private void Respawn()
	{
		//todo: generate random position
		movement.SpawnAt(transform.position + Vector3.up);
		stats.OnRespawn();
	}

	internal void DebugDie()
	{
		Die();
	}
}
