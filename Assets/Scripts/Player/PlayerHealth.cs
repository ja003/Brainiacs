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
		movement.SpawnAt(Vector2.up);
		stats.OnRespawn();
	}

	internal void DebugDie()
	{
		Die();
	}

	/// <summary>
	/// Damage from the collision and origin-player of the damage
	/// </summary>
	public bool OnCollision(int pDamage, Player pOwner, GameObject pOrigin)
	{
		if(IsDying)
		{
			Debug.Log("No kill for shooting dying player");
			return true;
		}

		ApplyDamage(pDamage, pOwner);

		return true;
	}

	/// <summary>
	/// Apply damage caused by pOrigin player.
	/// Origin can be null (map explosion, turret, ...) => TODO: test
	/// </summary>
	public void ApplyDamage(int pDamage, Player pOwner)
	{
		if(!player.IsInited || (pOwner != null && !pOwner.IsInited))
		{
			Debug.LogError($"Damage applied before player is inited. {player} | {pOwner}");
			return;
		}

		//owner doesnt have to be set (item explosion, ..)
		if(pOwner != null && !player.IsLocalImage) //effect would be applied 2* if local image
		{
			float damageMultiplier = pOwner.Stats.StatsEffect.GetDamageMultiplier();
			if(damageMultiplier != 1)
			{
				Debug.Log($"Multiply damage {pDamage} x {damageMultiplier}");
				pDamage = (int)(pDamage * damageMultiplier);
			}
		}

		//todo animation
		if(!player.IsItMe)
		{
			int playerNumber = pOwner != null ? pOwner.InitInfo.Number : -1;
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
				pOwner?.Stats.AddKill(forceAddKill);
			}
			//visual.OnDamage(); //visual effect first on owner then on image
		}
		//when image is hit => show visual effect => send info to owner => 
		// => apply damage => show visual effect on both sides
		visual.OnDamage();
	}
}
