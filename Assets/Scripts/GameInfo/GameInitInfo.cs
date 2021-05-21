using System;
using System.Collections.Generic;
using FlatBuffers;
using Photon.Pun;
using UnityEngine;

public class GameInitInfo
{
	public GameInitInfo()
	{
		Players = new List<PlayerInitInfo>();
		Mode = EGameMode.None;
		Map = EMap.None;
		GameModeValue = -1;
	}

	private EGameMode mode;
	public EGameMode Mode
	{
		get { return mode; }
		set { mode = value; /*OnInfoChanged();*/ }
	}

	public EMap Map { get; set; } = EMap.Steampunk;

	private int gameModeValue;
	public int GameModeValue
	{
		get { return gameModeValue; }
		set { gameModeValue = value; /*OnInfoChanged();*/ }
	}

	//todo: implement readonly structure -> prevent Players.Add call from outside
	public List<PlayerInitInfo> Players { get; }

	/// <summary>
	/// Add or update player info
	/// </summary>
	public void UpdatePlayer(PlayerInitInfo pInfo)
	{
		PlayerInitInfo existingPlayer = GetPlayer(pInfo.Number);
		if(existingPlayer != null)
		{
			existingPlayer.Update(pInfo);
		}
		else
		{
			Players.Add(pInfo);
		}
	}

	public override string ToString()
	{
		string playersTxt = "players: " + Environment.NewLine;
		foreach(var player in Players)
		{
			playersTxt += player + Environment.NewLine;
		}
		return $"InitInfo: P:{Players.Count}, M:{Mode},M:{Map},V:{GameModeValue} " +
			$"{Environment.NewLine} {playersTxt}";
	}

	//http://google.github.io/flatbuffers/flatbuffers_guide_tutorial.html
	internal byte[] Serialize()
	{
		FlatBufferBuilder fbb = new FlatBufferBuilder(1);

		//serialize players
		GameInitInfoS.StartPlayersVector(fbb, Players.Count);
		var playersOffset = new Offset<PlayerInitInfoS>[Players.Count];
		for(int i = 0; i < Players.Count; i++)
		{
			playersOffset[i] = Players[i].Create(ref fbb);
		}
		//nested tables needs to be CREATED before START
		VectorOffset playersVectorOffset = GameInitInfoS.CreatePlayersVector(fbb, playersOffset);

		//serialize whole game init info
		GameInitInfoS.StartGameInitInfoS(fbb);

		GameInitInfoS.AddMode(fbb, (int)Mode);
		GameInitInfoS.AddMap(fbb, (int)Map);
		GameInitInfoS.AddGameModeValue(fbb, GameModeValue);
		GameInitInfoS.AddPlayers(fbb, playersVectorOffset);

		Offset<GameInitInfoS> gameInfoOffset = GameInitInfoS.EndGameInitInfoS(fbb);
		fbb.Finish(gameInfoOffset.Value);
		byte[] result = fbb.SizedByteArray();
		return result;
	}

	internal static GameInitInfo Deserialize(GameInitInfoS pGameInfoS)
	{
		GameInitInfo gameInfo = new GameInitInfo();
		gameInfo.Mode = (EGameMode)pGameInfoS.Mode;
		gameInfo.Map = (EMap)pGameInfoS.Map;
		gameInfo.GameModeValue = pGameInfoS.GameModeValue;
		for(int i = 0; i < pGameInfoS.PlayersLength; i++)
		{
			PlayerInitInfoS? playerS = pGameInfoS.Players(i);
			if(playerS != null)
			{
				gameInfo.UpdatePlayer(PlayerInitInfo.Deserialize((PlayerInitInfoS)playerS));
			}
		}
		return gameInfo;
	}

	/// <summary>
	/// Set game values but not the info about players
	/// </summary>
	internal void SetGameValues(GameInitInfo pGameInfo)
	{
		mode = pGameInfo.mode;
		Map = pGameInfo.Map;
		GameModeValue = pGameInfo.GameModeValue;
	}

	//internal void UpdatePlayer(UIGameSetupPlayerEl pPlayerUi)
	//{
	//	PlayerInitInfo info = GetPlayer(pPlayerUi.Number);
	//	PlayerInitInfo newInfo = new PlayerInitInfo(pPlayerUi);

	//	bool playerRemoved = !pPlayerUi.gameObject.activeSelf;
	//	if(info == null)
	//	{
	//		if(!playerRemoved)
	//		{
	//			AddPlayer(newInfo);
	//			OnInfoChanged();
	//		}
	//		goto end;
	//	}
	//	else if(playerRemoved)
	//	{
	//		players.Remove(info);
	//	}
	//	//todo: info = newInfo doesnt change data in collection
	//	//find cleaner way
	//	info.Update(newInfo);

	//	end: OnInfoChanged();
	//}

	public PlayerInitInfo GetPlayer(int pPlayerNumber)
	{
		return Players.Find(a => a.Number == pPlayerNumber);
	}

	/// <summary>
	/// If there are some remote players.
	/// Note: call BrainiacsBehaviour::IsMultiplayer()
	/// </summary>
	public bool IsMultiplayer()
	{
		foreach(var p in Players)
		{
			if(p.PlayerType == EPlayerType.RemotePlayer)
				return true;
		}
		return false;
	}

	/// <summary>
	/// Am I master?
	/// If one of LocalPlayers has local photon then I have to be master.
	/// If single player => true.
	/// </summary>
	public bool IsMaster()
	{
		if(!IsMultiplayer())
			return true;

		foreach(var p in Players)
		{
			if(p.PlayerType == EPlayerType.LocalPlayer)
				return p.PhotonPlayer.IsLocal;
		}
		return false;
	}
}

