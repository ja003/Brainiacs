using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
	public int Lives;
	private const float default_speed = 1;
	public float Speed = default_speed;
	public int Health = 100;
	public int Kills;

	public EHero Hero;
	public string Name;

	public PlayerStats(PlayerInitInfo pPlayerInfo)
	{
		Lives = pPlayerInfo.Lives;
		Hero = pPlayerInfo.Hero;
		Name = pPlayerInfo.Name;
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


	/// <summary>
	/// Decreases health.
	/// Returns true if dead.
	/// </summary>
	public bool DecreseHealth(int pValue)
	{
		Debug.Log($"{Name} DecreseHealth by {pValue}");
		Health -= pValue;
		if(IsDead())
			Lives--;
		return IsDead();
	}
}
