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
	public float Speed { get; private set; } = DEFAULT_SPEED;
	public int Health { get; private set; } = 90;

	public int Kills { get; private set; }

	private bool isShielded;

	public PlayerInitInfo Info => player.InitInfo;

	private Action<PlayerStats> onStatsChange;

	[SerializeField] public Transform StatusUiPosition;

	public void SetOnStatsChange(Action<PlayerStats> pAction)
	{
		onStatsChange += pAction;
		onStatsChange.Invoke(this);
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
		OnRespawn(); //to update health etc
	}

	public void OnDie()
	{
		SetStat(EPlayerStats.Deaths, Deaths + 1);
	}

	public void AddKill(bool pForce)
	{
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



	//todo: implement effect management
	//if SetSpeed is called twice the first call will cancel effect
	//of the second one
	public void SetSpeed(float pSpeed, float pDuration)
	{
		Debug.Log($"{player.InitInfo.Name} set speed {pSpeed}");
		Speed = pSpeed;
		DoInTime(() => Speed = DEFAULT_SPEED, pDuration);
	}

	public void OnRespawn()
	{
		SetStat(EPlayerStats.Health, MAX_HEALTH);
	}

	public void AddHealth(int pIncrement, bool pRespawn = false)
	{
		if(IsDead && !pRespawn)
			return;

		if(pIncrement < 0 && isShielded)
		{
			Debug.Log($"{player.InitInfo.Name} is invurnelable");
			return;
		}

		SetStat(EPlayerStats.Health, Health + pIncrement);
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

		if(player.IsItMe)
		{
			switch(pType)
			{
				case EPlayerStats.Kills:
					Kills = pValue;
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

		//if(player.IsInited)
		//{
		//	//player image can only send info about kill - the rest
		//	//is handled at local player
		//	if(pType != EPlayerStats.Kills)
		//	{
		//		return;
		//	}

		//	player.Network.Send(EPhotonMsg.Player_AddKill, pForce);
		//}
	}

	public void SetShield(float pDuration)
	{
		isShielded = true;
		DoInTime(() => isShielded = false, pDuration);
	}
}

public enum EPlayerStats
{
	None,
	Kills,
	Health,
	Deaths
}
