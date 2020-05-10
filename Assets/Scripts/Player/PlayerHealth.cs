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

	public bool IsDying { get; private set; } //flag to prevent deadlock

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
		if(IsDying)
			return;

		IsDying = true;

		Debug.Log($"{this} Die ({stats.LivesLeft} lives left)");

		visual.OnDie();
		stats.OnDie();

		//controlled by animation now
		//if(stats.LivesLeft > 0)
		//{
		//	DoInTime(Respawn, 2);
		//}
	}

	public void OnDeadAnimFinished()
	{
		DoInTime(Respawn, 0.5f);
	}

	private void Respawn()
	{
		//todo: generate random position
		IsDying = false;
		movement.SpawnAt(Vector3.up);
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
		if(IsDying)
		{
			Debug.Log("No kill for shooting dying player");
			return true;
		}

		ApplyDamage(pDamage, pOrigin);

		return true;
	}

	/// <summary>
	/// Apply damage caused by pOrigin player.
	/// MAYBE: origin can be null (map explosion, turret, ...) => TODO: test
	/// </summary>
	public void ApplyDamage(int pDamage, Player pOrigin)
	{
		if(!player.IsInited || (pOrigin != null && !pOrigin.IsInited))
		{
			Debug.LogError($"Damage applied before player is inited. {player} | {pOrigin}");
			return;
		}

		//todo animation
		if(!player.IsItMe)
		{
			int playerNumber = pOrigin != null ? pOrigin.InitInfo.Number : -1;
			player.Photon.Send(EPhotonMsg.Player_ApplyDamage, pDamage, playerNumber);
		}
		else
		{
			if(IsDying)
			{
				//Cant apply damage to dying player;
				return;
			}

			bool wasGameEnded = game.GameEnd.GameEnded;
			stats.AddHealth(-pDamage);
			bool gameEndedAfterThis = game.GameEnd.GameEnded;

			//if this damage ended the game it has to be counted even after game ended
			bool forceAddKill = wasGameEnded != gameEndedAfterThis;

			if(stats.IsDead)
			{
				//Debug.Log("Add kill to " + pOrigin);
				pOrigin?.Stats.AddKill(forceAddKill);
			}
			//visual.OnDamage(); //visual effect first on owner then on image
		}
		//when image is hit => show visual effect => send info to owner => 
		// => apply damage => show visual effect on both sides
		visual.OnDamage();
	}
}
