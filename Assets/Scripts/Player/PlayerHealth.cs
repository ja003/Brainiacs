using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : GameBehaviour, IProjectileCollisionHandler
{
	[SerializeField]
	private PlayerStats stats;

	[SerializeField]
	private PlayerVisual visual;

	[SerializeField]
	private PlayerMovement movement;


	protected override void Awake()
	{
		base.Awake();
		stats.SetOnStatsChange(OnStatsChange);
	}

	private void OnStatsChange(PlayerStats pStats)
	{
		if(stats.IsDead())
		{
			Die();
		}
	}

	//public void OnHitBy(ProjectileConfig pConfig)
	//{
	//	if(stats.IsDead())
	//		return;

	//	if(stats.DecreseHealth(pConfig.damage))
	//	{
	//		todo: subscribe on stats change
	//		Die();
	//	}
	//}

	private void Die()
	{
		Debug.Log($"{this} Die ({stats.LivesLeft} lives left)");

		visual.OnDie();
		stats.Deaths++;

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

	public bool OnCollision(Projectile pProjectile)
	{
		stats.AddHealth(-pProjectile.config.Damage);
		return true;
	}
}
