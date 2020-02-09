using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats
{
	private int lives;
	public int Deaths;
	public int LivesLeft => lives - Deaths;

	public Color Color { get; internal set; }

	private const float default_speed = 1;
	public float Speed = default_speed;
	public int Health = 100;
	public int Kills;

	public EHero Hero;
	public string Name;

	private Action<PlayerStats> onStatsChange;

	public void SetOnStatsChange(Action<PlayerStats> pAction)
	{
		onStatsChange += pAction;
		onStatsChange.Invoke(this);
	}

	public PlayerStats(PlayerInitInfo pPlayerInfo)
	{
		lives = pPlayerInfo.Lives;
		Hero = pPlayerInfo.Hero;
		Name = pPlayerInfo.Name;
		Color = pPlayerInfo.Color;
	}

	internal bool IsDead()
	{
		return Health <= 0;
	}

	public void SetSpeed(float pSpeed, int pDuration)
	{
		Speed = pSpeed;
		//todo: duration
	}

	public void OnRespawn()
	{
		Health = 100;
	}

	public void AddKill()
	{
		Kills++;
		onStatsChange.Invoke(this);
	}


	/// <summary>
	/// Decreases health.
	/// Returns true if dead.
	/// </summary>
	public bool DecreseHealth(int pValue)
	{
		Debug.Log($"{Name} DecreseHealth by {pValue}");
		Health -= pValue;
		if(IsDead())
		{
			Deaths++;
		}
		onStatsChange.Invoke(this);
		return IsDead();
	}
}
