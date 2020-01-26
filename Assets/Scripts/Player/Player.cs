using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GameBehaviour
{
	public int Lives;
	public int Health;
	public int Kills;

	public EHero Hero;
	public string Name;

	internal void SetInfo(PlayerInitInfo playerInfo)
	{
		Lives = playerInfo.Lives;
		Hero = playerInfo.Hero;
		Name = playerInfo.Name;
	}
}
