using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : BrainiacsBehaviour, IProjectileCollisionHandler
{
	[SerializeField]
	private PlayerStats stats;

	[SerializeField]
	private PlayerVisual visual;

	[SerializeField]
	private PlayerMovement movement;

	Player player;

	protected override void Awake()
	{
		base.Awake();
		player = GetComponent<Player>();
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
		HitByProjectile(pProjectile.config.Damage);
		return true;
	}

	public void HitByProjectile(int pDamage)
	{
		//todo animation
		if(!player.IsItMe)
		{
			player.Network.Send(EPhotonMsg.Player_HitByProjectile, pDamage);
		}
		else
		{
			stats.AddHealth(-pDamage);
		}
	}
}
