using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : PlayerBehaviour, ICollisionHandler
{
	protected override void Awake()
	{
		stats.SetOnStatsChange(OnStatsChange);

		//InstantiateHealthbar();
		//Healthbar = InstanceFactory.Instantiate("p_Healthbar", Vector2.zero, false).GetComponent<UIHealthbar>();
		//Healthbar.Init(this, Vector2.up, true);


		base.Awake();
	}

	public bool IsDying { get; private set; } //flag to prevent deadlock

	//healthbar object is controlled from here (init, position) but value
	//is updated from UIPlayerInfoElement
	public UIHealthbar Healthbar;

	internal void Init()
	{
		InstantiateHealthbar();

		Color healthbarColor =
			brainiacs.PlayerColorManager.GetColor(player.InitInfo.Color);
			//player.InitInfo.PlayerType == EPlayerType.AI ? Color.blue : Color.green;
		Healthbar.Init(this, .75f * Vector2.up, true, healthbarColor);
	}

	private void InstantiateHealthbar()
	{
		if(Healthbar == null)
		{
			Healthbar = InstanceFactory.Instantiate("p_Healthbar", Vector2.zero, false).GetComponent<UIHealthbar>();
		}
	}

	private void OnStatsChange(PlayerStats pStats)
	{
		if(!player.IsInitedAndMe)
			return;

		if(stats.IsDead)
		{
			Die();
		}
	}

	/// <summary>
	/// Insta kill is used in multiplayer when user alt-tabs the app on phone.
	/// reason: game cant run on background and player is not responsive.
	/// When user opens game his player will be respawned.
	/// </summary>
	public bool IsInstaKilled;
	public void InstaKill()
	{
		Debug.Log("InstaKill");
		if(IsInstaKilled)
			return;

		IsInstaKilled = true;
		Die();
	}

	private void Die()
	{
		//Debug.Log("Die");
		if(IsDying)
			return;

		IsDying = true;

		Debug.Log($"{this} Die ({stats.LivesLeft} lives left)");

		visual.OnDie();
		stats.OnDie();
		weapon.OnDie();
	}

	public void OnDeadAnimFinished()
	{
		//Debug.Log("OnDeadAnimFinished");
		//if player was instakilled => show prompt
		if(IsInstaKilled)
		{
			Debug.Log($"Player {player} was insta killed => prompt");
			string warningPrompt = "You were killed because you closed the game during multiplayer." +
				Environment.NewLine + Environment.NewLine + "<b>DON'T DO THAT!</b>";
			game.Prompt.Show(warningPrompt, false, InstaRespawn);
			return;
		}
		DoInTime(Respawn, 0.5f);
	}

	public void InstaRespawn()
	{
		Debug.Log("InstaRespawn " + Time.time);
		if(!IsInstaKilled)
		{
			Debug.Log("no need to insta InstaRespawn");
			return;
		}
		Respawn();
		IsInstaKilled = false;
	}

	private void Respawn()
	{
		Debug.Log("Respawn");
		if(player.ai.IsTmp)
		{
			Debug.Log("AI dont respawn");
			return;
		}

		if(stats.LivesLeft <= 0)
		{
			Eliminate();
			return;
		}

		Vector2? respawnPos = game.Map.ActiveMap.GetRandomPosition();
		if(respawnPos == null)
		{
			Debug.Log("Couldnt find good spawn position => spawn at center");
			respawnPos = Vector2.zero;
		}

		//todo: generate random position
		IsDying = false;
		movement.SpawnAt((Vector2)respawnPos);
		stats.OnRespawn();
	}

	/// <summary>
	/// No life left => dont respawn
	/// </summary>
	private void Eliminate()
	{
		DoEliminateEffect();
	}

	/// <summary>
	/// Sound. todo: maybe animation on scoreboard?
	/// RPC
	/// </summary>
	public void DoEliminateEffect()
	{
		Debug.Log($"Player {player} is OUT!");
		brainiacs.AudioManager.PlaySound(ESound.Player_Eliminate, null);
		player.Photon.Send(EPhotonMsg.Player_DoEliminateEffect);
		game.InfoMessenger.Show($"Player {player.InitInfo.GetName(true)} has been <b>eliminated!</b>");
	}

	internal void DebugDie()
	{
		Die();
	}

	//ref to DaVinco tank.
	//needed for multiplayer because player image doesnt have Active weapon set
	//so there would be no way to check the tank state
	public SpecialDaVinciTank MyTank;

	/// <summary>
	/// Damage from the collision and origin-player of the damage
	/// </summary>
	public bool OnCollision(int pDamage, Player pOwner, GameObject pOrigin, Vector2 pPush)
	{
		if(IsDying)
		{
			//Debug.Log("No kill for shooting dying player");
			return true;
		}

		if(MyTank != null && MyTank.gameObject.activeSelf)
		{
			//Debug.Log("Redirect collision to tank");
			//todo: not very nice implementation
			//- this happens when 
			//-- 2 DaVinci tanks collide
			//-- Einstein bomb => lets keep this (tank gets 2 hits from the bomb)
			//-- other collisions should be handled by the tank and shouldnt reach player
			//- try figure out how 2 tanks can collide
			return MyTank.OnCollision(pDamage, pOwner, pOrigin, pPush);
		}

		player.Push.Push(pPush);

		ApplyDamage(pDamage, pOwner);

		return true;
	}

	//Tesla clone has special health. he will be killed in 'CloneHealth' hits
	public int CloneHealth = 3; 

	/// <summary>
	/// Apply damage caused by pOrigin player.
	/// Origin can be null (map explosion, turret, ...) => TODO: test
	/// </summary>
	public void ApplyDamage(int pDamage, Player pOrigin)
	{
		if(debug.InstaKill)
			pDamage = PlayerStats.MAX_HEALTH;

		//clone has special damage system
		if(player.ai.IsTmp)
		{
			pDamage = (int)Mathf.Ceil(PlayerStats.MAX_HEALTH / (float)CloneHealth);
		}

		if(!player.IsInited || (pOrigin != null && !pOrigin.IsInited))
		{
			Debug.LogError($"Damage applied before player is inited. {player} | {pOrigin}");
			return;
		}

		

		//owner doesnt have to be set (item explosion, ..)
		if(pOrigin != null && !player.IsLocalImage) //effect would be applied 2* if local image
		{
			float damageMultiplier = pOrigin.Stats.StatsEffect.GetDamageMultiplier();
			if(damageMultiplier != 1)
			{
				Debug.Log($"Multiply damage {pDamage} x {damageMultiplier}");
				pDamage = (int)(pDamage * damageMultiplier);
			}
		}

		//when image is hit => send info to owner => apply damage
		if(!player.IsItMe)
		{
			int playerNumber = pOrigin != null ? pOrigin.InitInfo.Number : -1;
			player.Photon.Send(EPhotonMsg.Player_ApplyDamage, pDamage, playerNumber);
			return;
		}


		if(IsDying)
		{
			//Cant apply damage to dying player;
			return;
		}

		bool wasGameEnded = game.GameEnd.GameEnded;
		int res = stats.AddHealth(-pDamage);
		if(res < 0)
		{
			//show damage effect only if some damage was applied (can be blocked by shield)
			OnReceiveDamageEffect();
		}

		bool gameEndedAfterThis = game.GameEnd.GameEnded;

		//if this damage ended the game it has to be counted even after game ended
		bool forceAddKill = wasGameEnded != gameEndedAfterThis;

		//no kill point for killing AI
		if(stats.IsDead && !player.ai.IsTmp)
		{
			//Debug.Log("Add kill to " + pOrigin);
			pOrigin?.Stats.AddKill(forceAddKill);
			string message = pOrigin == null ?
				$"Player {player.InitInfo.GetName(true)} was <b>killed</b>" :
				$"Player {player.InitInfo.GetName(true)} <b>killed</b> by {pOrigin.InitInfo.GetName(true)}";
			game.InfoMessenger.Show(message);
		}
		//visual.OnDamage(); //visual effect first on owner then on image
	}

	/// <summary>
	/// Visual + sound effect after some damage has been received.
	/// RPC - owner sends
	/// </summary>
	public void OnReceiveDamageEffect()
	{
		visual.OnDamage();
		const float max_hit_sound_freq = 0.2f;
		PlaySound(ESound.Player_Hit, max_hit_sound_freq);
		player.Photon.Send(EPhotonMsg.Player_OnReceiveDamageEffect);
	}

	internal void OnReturnToPool()
	{
		//needed for Tesla clone
		IsDying = false;
	}
}
