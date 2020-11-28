using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Player score UI.
/// Changes on mine player stats change and sends data to others
/// </summary>
public class UIScoreboardElement : GameBehaviour
{
	[SerializeField] private TextMeshProUGUI playerName = null;
	[SerializeField] private Text kills = null;
	[SerializeField] private Text deaths = null;
	[SerializeField] private Image background = null;

	Player player;

	/// <summary>
	/// Called after all players are added
	/// </summary>
	public void Init(Player pPlayer)
	{
		player = pPlayer;

		//color and name dont change during game
		background.color = UIColorDB.GetColor(pPlayer.InitInfo.Color);
		//todo: sortingOrder is not applied -> set it manually
		background.GetComponent<Canvas>().sortingOrder = -1;
		playerName.text = pPlayer.InitInfo.Name;

		if(pPlayer.IsItMe)
		{
			//register for stats change
			pPlayer.Stats.SetOnStatsChange(OnStatsChanged);
			//invoke first change manually
			OnStatsChanged(pPlayer.Stats);
		}

		pPlayer.Visual.Scoreboard = this;
		game.Lighting.RegisterForLighting(background);
	}

	private void OnStatsChanged(PlayerStats pStats)
	{
		SetScore(pStats.Kills, pStats.Deaths);
	}

	public void SetScore(int pKills, int pDeaths)
	{
		kills.text = pKills.ToString();
		deaths.text = pDeaths.ToString();
		player.Photon.Send(EPhotonMsg.Player_UI_Scoreboard_SetScore, pKills, pDeaths);
	}
}
