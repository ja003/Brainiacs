using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : PlayerBehaviour, ICollisionHandler
{
	protected override void Awake()
	{
		stats.SetOnStatsChange(OnStatsChange);
		base.Awake();
	}

	bool isDying; //flag to prevent deadlock
	private void OnStatsChange(PlayerStats pStats)
	{
		if(!player.IsInited || !player.IsItMe)
			return;

		if(stats.IsDead)
		{
			Die();
		}
	}


	private void Die()
	{
		if(isDying)
			return;

		isDying = true;

		Debug.Log($"{this} Die ({stats.LivesLeft} lives left)");

		visual.OnDie();
		stats.OnDie();

		if(stats.LivesLeft > 0)
		{
			DoInTime(Respawn, 2);
		}
	}

	private void Respawn()
	{
		//todo: generate random position
		isDying = false;
		movement.SpawnAt(transform.position + Vector3.up);
		stats.OnRespawn();
	}

	internal void DebugDie()
	{
		Die();
	}

	/// <summary>
	/// Damage from the collision and origin-player of the damage
	/// </summary>
	public bool OnCollision(int pDamage, Player pOrigin)
	{
		if(isDying)
		{
			Debug.Log("No kill for shooting dying player");
			return true;
		}

		ApplyDamage(pDamage, pOrigin);

		return true;
	}

	public void ApplyDamage(int pDamage, Player pOrigin)
	{
		//todo animation
		if(!player.IsItMe)
		{
			player.Photon.Send(EPhotonMsg.Player_ApplyDamage, pDamage, pOrigin.InitInfo.Number);
		}
		else
		{
			bool wasGameEnded = game.GameEnd.GameEnded;
			stats.AddHealth(-pDamage);
			bool gameEndedAfterThis = game.GameEnd.GameEnded;
			bool forceAddKill = wasGameEnded != gameEndedAfterThis;

			if(stats.IsDead)
				pOrigin.Stats.AddKill(forceAddKill);
		}
	}
}
