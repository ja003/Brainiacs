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
		if(isMultiplayer && !PhotonNetwork.IsMasterClient)
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

	public void CheckEndGame(PlayerScoreInfo pResult)
	{
		if(debug.DontEndGame)
			return;

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
				if(alivePlayersCount <= 1 && !GameEnded)
					EndGame();

				break;
			default:
				break;
		}
	}

	public Action OnGameEnd;

	/// <summary>
	/// End game - called only on master.
	/// Block user input and prevents player stats from changing
	/// </summary>
	public void EndGame()
	{
		if(GameEnded)
		{
			Debug.LogError("EndGame called again");
			return;
		}

		if(isMultiplayer && !PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("EndGame should be called only on master");
			return;
		}



		//generate from players stats
		brainiacs.SetGameResultInfo(game.Results.PlayersScore, game.GameTime.TimePassed);

		//send result info
		var gameResultBytes = brainiacs.GameResultInfo.Serialize();
		game.Photon.Send(EPhotonMsg.Game_EndGame, gameResultBytes);

		//every user will load Results scene locally
		//game.uiCurtain.SetFade(true, LoadResultScene, 2);

		//in deathmatch/score game start end-game-fade cca after last death animation is finished
		int fadeDelay = brainiacs.GameInitInfo.Mode != EGameMode.Time ? 2 : 0;
		const int fadeTime = 2;
		DoInTime(() =>
		{
			game.uiCurtain.SetFade(true, LoadResultScene, fadeTime);
		}, fadeDelay, true);

		game.InfoMessenger.Show("Game ended"); //has to be called before GameEnded flag is set
		GameEnded = true; //prevents player input and stats change
		OnGameEnd?.Invoke();
		Debug.Log("EndGame");
	}



	/// <summary>
	/// End game - called only on clients
	/// </summary>
	public void OnReceiveEndGame(GameResultInfo pGameResultInfo)
	{
		Debug.Log("OnReceiveEndGame");
		//has to be called before GameEnded flag is set
		game.InfoMessenger.Show("This game is finished!"); 
		GameEnded = true;
		brainiacs.GameResultInfo = pGameResultInfo;

		DoInTime(() => game.uiCurtain.SetFade(true, LoadResultScene), 2);
	}

	private void LoadResultScene()
	{
		//game could have been paused
		game.GameTime.SetPause(false);

		if(isMultiplayer)
			PhotonNetwork.LeaveRoom();
		brainiacs.Scenes.LoadScene(EScene.Results);
	}

}
