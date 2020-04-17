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
		if(!player.IsInitedAndMe)
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
		if(!player.IsInited || !pOrigin.IsInited)
		{
			Debug.LogError($"Damage applied before player is inited. {player} | {pOrigin}");
			return;
		}

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

			//if this damage ended the game it has to be counted even after game ended
			bool forceAddKill = wasGameEnded != gameEndedAfterThis;

			if(stats.IsDead)
			{
				//Debug.Log("Add kill to " + pOrigin);
				pOrigin.Stats.AddKill(forceAddKill);
			}
		}
	}
}
