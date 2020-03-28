using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreboardElement : BrainiacsBehaviour
{
	[SerializeField] private TextMeshProUGUI name;
	[SerializeField] private Text kills;
	[SerializeField] private Text deaths;
	[SerializeField] private Image background;

	Player player;
	public void Init(Player pPlayer)
	{
		player = pPlayer;

		//color and name dont change during game
		background.color = UIColorDB.GetColor(pPlayer.Stats.Color);
		//todo: sortingOrder is not applied -> set it manually
		background.GetComponent<Canvas>().sortingOrder = -1;
		name.text = pPlayer.Stats.Name;

		//register for stats change
		pPlayer.Stats.SetOnStatsChange(OnStatsChanged);
		//invoke first change manually
		OnStatsChanged(pPlayer.Stats);

		pPlayer.Visual.Scoreboard = this;
	}

	private void OnStatsChanged(PlayerStats pStats)
	{
		SetStats(pStats.Kills, pStats.Deaths);
	}

	public void SetStats(int pKills, int pDeaths)
	{
		kills.text = pKills.ToString();
		deaths.text = pDeaths.ToString();
		player.Network.Send(EPhotonMsg.Player_UI_Scoreboard_SetStats, pKills, pDeaths);

	}
}
