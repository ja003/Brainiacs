using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : PlayerBehaviour
{
	private int lives;
	public int Deaths { get; private set; }
	public int LivesLeft => lives - Deaths;

	public bool IsDead => Health <= 0;

	//public EPlayerColor Color { get; internal set; }

	private const float DEFAULT_SPEED = 1;
	public const int MAX_HEALTH = 100;
	public float Speed { 
		get {
			return DEFAULT_SPEED 
				+ StatsEffect.GetEffectValue(EPlayerEffect.DoubleSpeed)
				- StatsEffect.GetEffectValue(EPlayerEffect.HalfSpeed);
		}
	}
	public int Health { get; private set; } = 90;

	public int Kills { get; private set; }

	public bool IsShielded => StatsEffect.GetEffectValue(EPlayerEffect.Shield) > 0
		|| (debug.Shield 
		&& player.InitInfo.PlayerType == EPlayerType.LocalPlayer); //test only on me

	public PlayerInitInfo Info => player.InitInfo;

	private Action<PlayerStats> onStatsChange;

	//position where info about picked up item will show
	[SerializeField] public Transform MapItemUiPosition;
	//change of health info
	[SerializeField] public Transform HealthUiPosition;

	[SerializeField] public PlayerStatsEffectController StatsEffect;

	public void SetOnStatsChange(Action<PlayerStats> pAction)
	{
		onStatsChange += pAction;
		onStatsChange.Invoke(this);
	}

	private float GetSpeed()
	{
		return DEFAULT_SPEED;
	}

	public void Init()
	{
		//Info = pPlayerInfo;
		GameInitInfo gameInitInfo = brainiacs.GameInitInfo;
		lives = gameInitInfo.Mode == EGameMode.Deathmatch ?
			gameInitInfo.GameModeValue : 666;

		//SetStat(EPlayerStats.Health, MAX_HEALTH);
		//Hero = pPlayerInfo.Hero;
		//Name = pPlayerInfo.GetName();
		//Color = pPlayerInfo.Color;

		player.OnPlayerInited.AddAction(OnRespawn); //stats can be set only after player is inited
													//OnRespawn(); //to update health etc
	}

	public void OnDie()
	{
		SetStat(EPlayerStats.Deaths, Deaths + 1);
		StatsEffect.OnDie();
	}

	public void AddKill(bool pForce)
	{
		//Debug.Log("AddKill");

		if(!player.IsInited)
		{
			Debug.LogError($"AddKill called before player is inited. {player}");
			return;
		}

		//if is Tesla clone => add kill to owner of the clone
		if(player.ai.IsTmp)
		{
			player.ai.Owner.Stats.AddKill(pForce);
			return;
		}

		//stats set only at local player
		if(player.IsItMe)
		{
			SetStat(EPlayerStats.Kills, Kills + 1, pForce);
		}
		//player image only sends info 
		else
		{
			//Debug.Log("send AddKill");
			player.Photon.Send(EPhotonMsg.Player_AddKill, pForce);
		}
	}

	public void OnRespawn()
	{
		//Debug.Log($"OnRespawn - {player}");
		SetStat(EPlayerStats.Health, MAX_HEALTH);
	}

	/// <summary>
	/// Returns the amount of actualy increased health .
	/// Can be nulled by shield
	/// </summary>
	/// <returns></returns>
	public int AddHealth(int pIncrement, bool pRespawn = false)
	{
		if(IsDead && !pRespawn)
			return 0;

		if(pIncrement < 0 && (IsShielded || debug.Invulnerability))
		{
			if(IsShielded)
			{
				DoShieldHitEffect();
			}
			//Debug.Log($"{player.InitInfo.GetName()} is invurnelable");
			return 0;
		}

		SetStat(EPlayerStats.Health, Health + pIncrement);
		if(!pRespawn)
			game.PlayerStatusManager.ShowHealth(player.Stats.HealthUiPosition.position, pIncrement);

		return pIncrement;
	}

	public void DoShieldHitEffect()
	{
		const float max_play_sound_freq = 0.2f;
		PlaySound(ESound.Player_Shield_Hit, max_play_sound_freq);
		player.Photon.Send(EPhotonMsg.Player_DoShieldHitEffect);
	}

	public void SetStat(EPlayerStats pType, int pValue, bool pForce = false)
	{
		//Debug.Log($"Set stat {pType} = {pValue}");

		if(game.GameEnd.GameEnded && !pForce)
		{
			Debug.Log("GAME ENDED");
			return;
		}

		if(pForce)
		{
			Debug.Log("Set stat even after game end");
		}

		if(!player.IsInited)
		{
			Debug.LogError($"SetStat called before player is inited. {player}");
			return;
		}

		if(player.IsItMe)
		{
			switch(pType)
			{
				case EPlayerStats.Kills:
					Kills = pValue;
					//Debug.Log(player + " Kills = " + Kills);
					break;
				case EPlayerStats.Health:
					int newHealthValue = Mathf.Clamp(pValue, debug.Immortality ? 1 : 0, MAX_HEALTH);
					Health = newHealthValue;
					player.Health.Healthbar?.SetHealth(Health, MAX_HEALTH);
					player.Photon.Send(EPhotonMsg.Player_UpdateHealthbar, Health);
					break;
				case EPlayerStats.Deaths:
					Deaths = pValue;
					break;
			}

			onStatsChange?.Invoke(this);

			//local player doesnt send stat info to its image
			return;
		}
	}

	internal void OnReturnToPool()
	{
		StatsEffect.OnReturnToPool();
		SetStat(EPlayerStats.Health, 0);//, true); //set as dead
	}
}

public enum EPlayerStats
{
	None,
	Kills,
	Health,
	Deaths
}
