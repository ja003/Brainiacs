﻿using System;
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

	public PlayerResultInfo(Player pPlayer)
	{
		Name = pPlayer.Name;
		Hero = pPlayer.Hero;
		Kills = pPlayer.Kills;
		LivesLeft = pPlayer.Lives;
	}

	public int GetScore()
	{
		return Kills + LivesLeft;
	}
}