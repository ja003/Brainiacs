using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitInfo
{
	public GameInitInfo()
	{
		players = new List<PlayerInitInfo>();
		Mode = EGameMode.None;
		Map = EMap.None;
		Time = -1;
		Lives = -1;
		DeathMatchGoal = -1;
	}

	public List<PlayerInitInfo> players;
	public EGameMode Mode;
	public EMap Map;
	public int Time;
	public int Lives;
	public int DeathMatchGoal;
}


public class PlayerInitInfo
{
	public string Name;
	public EHero Hero;
	public int Lives;
	public PlayerKeys PlayerKeys;
	//public Color Color;
	public EPlayerColor Color;

	public PlayerInitInfo(EHero pHero, string pName, EPlayerColor pColor)
	{
		Hero = pHero;
		Name = pName;
		Color = pColor;

		Lives = 10;
	}

}

public struct PlayerKeys
{
	public KeyCode moveUp;
	public KeyCode moveRight;
	public KeyCode moveDown;
	public KeyCode moveLeft;
	
	public KeyCode useWeapon;
	public KeyCode swapWeapon;

	public PlayerKeys(KeyCode moveUp, KeyCode moveRight, KeyCode moveDown, KeyCode moveLeft, KeyCode useWeapon, KeyCode swapWeapon)
	{
		this.moveUp = moveUp;
		this.moveRight = moveRight;
		this.moveDown = moveDown;
		this.moveLeft = moveLeft;
		this.useWeapon = useWeapon;
		this.swapWeapon = swapWeapon;
	}
}

public enum EGameMode
{
	None,
	Time,
	Score,
	Deathmatch
}

public enum EMap
{
	None,
	Steampunk,
	Wonderland
}

public enum EHero
{
	None = 0,
	Currie = 1,
	DaVinci = 2,
	Einstein = 3,
	Nobel = 4,
	Tesla = 5,
}

public enum EPlayerColor
{
	Blue = 0,
	Red = 1,
	Yellow = 2,
	Green = 3,
	Pink = 4,
}