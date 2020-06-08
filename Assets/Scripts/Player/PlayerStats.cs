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
	private const int MAX_HEALTH = 100;
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
		|| (DebugData.TestShield 
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
		//Hero = pPlayerInfo.Hero;
		//Name = pPlayerInfo.Name;
		//Color = pPlayerInfo.Color;

		player.OnPlayerInited.AddAction(OnRespawn); //stats can be set only after player is inited
													//OnRespawn(); //to update health etc
	}

	public void OnDie()
	{
		SetStat(EPlayerStats.Deaths, Deaths + 1);
	}

	public void AddKill(bool pForce)
	{
		if(!player.IsInited)
		{
			Debug.LogError($"AddKill called before player is inited. {player}");
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
			player.Photon.Send(EPhotonMsg.Player_AddKill, pForce);
		}
	}




	public void OnRespawn()
	{
		SetStat(EPlayerStats.Health, MAX_HEALTH);
	}

	public void AddHealth(int pIncrement, bool pRespawn = false)
	{
		if(IsDead && !pRespawn)
			return;

		if(pIncrement < 0 && (IsShielded || DebugData.TestImmortality))
		{
			//Debug.Log($"{player.InitInfo.Name} is invurnelable");
			return;
		}

		SetStat(EPlayerStats.Health, Health + pIncrement);
		if(!pRespawn)
			game.PlayerStatusManager.ShowHealth(player.Stats.HealthUiPosition.position, pIncrement);
	}


	public void SetStat(EPlayerStats pType, int pValue, bool pForce = false)
	{
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
					Health = Mathf.Clamp(pValue, 0, MAX_HEALTH);
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

}

public enum EPlayerStats
{
	None,
	Kills,
	Health,
	Deaths
}
