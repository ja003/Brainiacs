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

	public Player(int lives, EHero hero, string name)
	{
		Lives = lives;
		Hero = hero;
		Name = name;
	}
}
