using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndController : GameController
{
	EGameMode mode => brainiacs.GameInitInfo.Mode;

	public bool GameEnded { get; private set; }


	protected override void OnMainControllerAwaken()
	{
		game.PlayerManager.OnAllPlayersAdded.AddAction(Init);
	}

	private void Init()
	{
		if(brainiacs.GameInitInfo.IsMultiplayer() && !PhotonNetwork.IsMasterClient)
		{
			//only master checks end of game
			return;
		}

		if(mode != EGameMode.Time)
		{
			game.Results.OnResultUpdated += CheckEndGame;
			//foreach(var player in game.PlayerManager.Players)
			//{
			//	player.Stats.SetOnStatsChange(CheckEndGame);
			//}
		}
	}

	private void CheckEndGame(PlayerScoreInfo pResult)
	{
		switch(mode)
		{
			case EGameMode.Time:
				break;
			case EGameMode.Score:
				if(pResult.Kills >= brainiacs.GameInitInfo.GameModeValue)
					EndGame();

				break;
			case EGameMode.Deathmatch:
				int alivePlayersCount = 0;
				foreach(var player in game.PlayerManager.Players)
				{
					if(player.Stats.LivesLeft > 0)
						alivePlayersCount++;
				}
				if(alivePlayersCount <= 1)
					EndGame();

				break;
			default:
				break;
		}
	}

	/// <summary>
	/// End game - called only on master.
	/// Block user input and prevents player stats from changing
	/// </summary>
	public void EndGame()
	{
		if(brainiacs.GameInitInfo.IsMultiplayer() && !PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("EndGame should be called only on master");
			return;
		}

		GameEnded = true;
		Debug.Log("EndGame");

		//generate from players stats
		brainiacs.SetGameResultInfo(game.Results.PlayersScore, game.GameTime.TimePassed);

		//send result info
		var gameResultBytes = brainiacs.GameResultInfo.Serialize();
		game.Photon.Send(EPhotonMsg.Game_EndGame, gameResultBytes);


		//every user will load Results scene locally
		game.uiCurtain.SetFade(true, LoadResultScene);
	}

	

	/// <summary>
	/// End game - called only on clients
	/// </summary>
	public void OnReceiveEndGame(GameResultInfo pGameResultInfo)
	{
		GameEnded = true;
		brainiacs.GameResultInfo = pGameResultInfo;
		game.uiCurtain.SetFade(true, LoadResultScene);
	}

	private void LoadResultScene()
	{
		if(brainiacs.GameInitInfo.IsMultiplayer())
			PhotonNetwork.LeaveRoom();
		brainiacs.Scenes.LoadScene(EScene.Results);
	}

}
