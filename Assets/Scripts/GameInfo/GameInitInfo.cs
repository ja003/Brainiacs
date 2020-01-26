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

	public PlayerInitInfo(EHero pHero, string pName)
	{
		Hero = pHero;
		Name = pName;
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