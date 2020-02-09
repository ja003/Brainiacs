using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultInfo
{
	public List<PlayerResultInfo> PlayerResults;

	public GameResultInfo()
	{
		PlayerResults = new List<PlayerResultInfo>();
	}

	internal PlayerResultInfo GetResultInfo(int pPosition)
	{
		if(pPosition > PlayerResults.Count)
			return null;
		PlayerResults.Sort((a, b) => a.GetScore().CompareTo(b.GetScore()));
		return PlayerResults[pPosition - 1];
	}
}

public class PlayerResultInfo
{
	public string Name;
	public EHero Hero;
	public int Kills;
	public int LivesLeft;

	public PlayerResultInfo(PlayerStats pStats)
	{
		Name = pStats.Name;
		Hero = pStats.Hero;
		Kills = pStats.Kills;
		LivesLeft = pStats.LivesLeft;
	}

	public int GetScore()
	{
		return Kills + LivesLeft;
	}
}
