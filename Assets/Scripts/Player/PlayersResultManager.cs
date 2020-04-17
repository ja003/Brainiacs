using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds players results.
/// Only on master
/// </summary>
public class PlayersResultManager : GameController
{
	public List<PlayerScoreInfo> PlayersScore;

	protected override void OnMainControllerAwaken()
	{
		game.PlayerManager.OnAllPlayersAdded.AddAction(Init);
	}

	/// <summary>
	/// Called after all players are added (and inited)
	/// </summary>
	private void Init()
	{
		PlayersScore = new List<PlayerScoreInfo>();
		foreach(var player in game.PlayerManager.Players)
		{
			if(player.IsLocalImage)
				continue;
			PlayersScore.Add(new PlayerScoreInfo(player.Stats));

			//local player send score info to master
			if(player.IsItMe)
				player.Stats.SetOnStatsChange(UpdateResult);
		}
	}

	public Action<PlayerScoreInfo> OnResultUpdated;

	private void UpdateResult(PlayerStats pStat)
	{
		//Debug.Log($"UpdateResult player: {pStat.Info.Number}");
		UpdatePlayerScore(pStat.Info.Number, pStat.Kills, pStat.Deaths);
	}

	//todo: maybe connect this to scoreboard?
	public void UpdatePlayerScore(int pPlayerNumber, int pKills, int pDeaths)
	{
		PlayerScoreInfo result = PlayersScore.Find(a => a.PlayerNumber == pPlayerNumber);
		bool updated = result.UpdateScore(pKills, pDeaths);
		if(updated)
		{
			OnResultUpdated?.Invoke(result);
			//send target = master
			game.Photon.Send(EPhotonMsg.Game_UpdatePlayerScore, pPlayerNumber, pKills, pDeaths);
		}
	}
}

