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

	public PlayerInitInfo(EHero pHero, string pName)
	{
		Hero = pHero;
		Name = pName;
	}

}

public struct PlayerKeys
{
	public KeyCode moveUp;
	public KeyCode moveRight;
	public KeyCode moveDown;
	public KeyCode moveLeft;
	
	public KeyCode useItem;
	public KeyCode swapItem;

	public PlayerKeys(KeyCode moveUp, KeyCode moveRight, KeyCode moveDown, KeyCode moveLeft, KeyCode useItem, KeyCode swapItem)
	{
		this.moveUp = moveUp;
		this.moveRight = moveRight;
		this.moveDown = moveDown;
		this.moveLeft = moveLeft;
		this.useItem = useItem;
		this.swapItem = swapItem;
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
	None,
	Einstein,
	Currie,
	Tesla,
	Edison
}