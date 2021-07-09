using FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreInfo
{
	public int PlayerNumber;
	public string Name;
	public EHero Hero;
	public int Kills;
	public int Deaths; //todo: regenerate flatbuffer

	private PlayerScoreInfo() { }

	public static PlayerScoreInfo debug_PlayerResultInfo() { return new PlayerScoreInfo(); }

	public PlayerScoreInfo(PlayerStats pStats)
	{
		PlayerNumber = pStats.Info.Number;
		Name = pStats.Info.GetName();
		Hero = pStats.Info.Hero;
		Kills = pStats.Kills;
		Deaths = pStats.Deaths;
	}

	public int GetScore()
	{
		return Kills - Deaths;
	}

	internal Offset<PlayerResultInfoS> Create(ref FlatBufferBuilder fbb)
	{
		StringOffset nameOff = fbb.CreateString(Name);

		Offset<PlayerResultInfoS> result = PlayerResultInfoS.CreatePlayerResultInfoS(fbb,
			PlayerNumber, nameOff, (int)Hero, Kills, Deaths);
		return result;
	}

	/// <summary>
	/// Updates only kills and deaths.
	/// If no value was changed => returns false
	/// </summary>
	internal bool UpdateScore(int pKills, int pDeaths)
	{
		if(Kills == pKills && Deaths == pDeaths)
			return false;

		Kills = pKills;
		Deaths = pDeaths;
		return true;
	}

	internal static PlayerScoreInfo Deserialize(PlayerResultInfoS pResultS)
	{
		PlayerScoreInfo playerResultInfo = new PlayerScoreInfo();
		playerResultInfo.PlayerNumber = pResultS.Number;
		playerResultInfo.Name = pResultS.Name;
		playerResultInfo.Hero = (EHero)pResultS.Hero;
		playerResultInfo.Kills = pResultS.Kills;
		playerResultInfo.Deaths = pResultS.LivesLeft; //todo: regenerate flatbuffer

		return playerResultInfo;
	}

	public override string ToString()
	{
		return $"Result: P[{PlayerNumber}], {Kills}/{Deaths}";
	}
}
