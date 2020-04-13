using FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultInfo
{
	public int PlayTime;
	public List<PlayerScoreInfo> PlayerResults;

	public GameResultInfo()
	{
		PlayerResults = new List<PlayerScoreInfo>();
	}

	internal PlayerScoreInfo GetResultInfo(int pPosition)
	{
		if(pPosition > PlayerResults.Count)
			return null;

		//sort descending
		PlayerResults.Sort((a, b) => b.GetScore().CompareTo(a.GetScore()));
		return PlayerResults[pPosition - 1];
	}

	internal byte[] Serialize()
	{
		FlatBufferBuilder fbb = new FlatBufferBuilder(1);

		//serialize players
		GameResultInfoS.StartPlayerResultsVector(fbb, PlayerResults.Count);
		var playersOffset = new Offset<PlayerResultInfoS>[PlayerResults.Count];
		for(int i = 0; i < PlayerResults.Count; i++)
		{
			playersOffset[i] = PlayerResults[i].Create(ref fbb);
		}
		//nested tables needs to be CREATED before START
		VectorOffset playersVectorOffset = GameResultInfoS.CreatePlayerResultsVector(fbb, playersOffset);

		//serialize whole game init info
		GameResultInfoS.StartGameResultInfoS(fbb);
		GameResultInfoS.AddPlayTime(fbb, PlayTime);
		GameResultInfoS.AddPlayerResults(fbb, playersVectorOffset);

		Offset<GameResultInfoS> gameResultInfoOffset = GameResultInfoS.EndGameResultInfoS(fbb);
		fbb.Finish(gameResultInfoOffset.Value);
		byte[] result = fbb.SizedByteArray();
		return result;
	}

	internal static GameResultInfo Deserialize(GameResultInfoS pInfoS)
	{
		GameResultInfo gameResultInfo = new GameResultInfo();
		gameResultInfo.PlayTime = pInfoS.PlayTime;
		for(int i = 0; i < pInfoS.PlayerResultsLength; i++)
		{
			PlayerResultInfoS? playerS = pInfoS.PlayerResults(i);
			if(playerS != null)
			{
				gameResultInfo.PlayerResults.Add(PlayerScoreInfo.Deserialize((PlayerResultInfoS)playerS));
			}
		}
		return gameResultInfo;
	}
}

