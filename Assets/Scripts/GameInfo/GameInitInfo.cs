using System;
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
		GameModeValue = -1;
	}

	public List<PlayerInitInfo> players;
	public EGameMode Mode;
	public EMap Map;
	public int GameModeValue;

}


public class PlayerInitInfo
{
	public int Number;
	public string Name;
	public EHero Hero;
	public int Lives;
	//public PlayerKeys PlayerKeys;
	public EPlayerColor Color;
	public EPlayerType PlayerType;

	public PlayerInitInfo(int pNumber, EHero pHero, string pName, EPlayerColor pColor, EPlayerType pPlayerType)
	{
		Number = pNumber;
		Hero = pHero;
		Name = pName;
		Color = pColor;
		PlayerType = pPlayerType;
		Lives = 10;
	}

}

[Serializable]
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
	None = 0,
	Blue = 1,
	Red = 2,
	Yellow = 3,
	Green = 4,
	Pink = 5,
}