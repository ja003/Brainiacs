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

	[SerializeField]
	private PlayerInput input;

	

	internal void SetInfo(PlayerInitInfo pPlayerInfo)
	{
		Lives = pPlayerInfo.Lives;
		Hero = pPlayerInfo.Hero;
		Name = pPlayerInfo.Name;
		
		input.Keys = pPlayerInfo.PlayerKeys;

	}


}
