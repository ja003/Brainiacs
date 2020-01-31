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

	public void SetSpeed(float pSpeed, int pDuration)
	{
		Speed = pSpeed;
		//todo: duration
	}
}
