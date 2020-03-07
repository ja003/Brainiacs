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

	//public void Init(PlayerInitInfo pPlayerInfo)
	//{
	//	name.text = pPlayerInfo.Name;
	//	kills.text = "0";
	//	deaths.text = "0";

	//	image.color = pPlayerInfo.Color;

	//	gameObject.SetActive(true);
	//}

	public void Init(PlayerStats pStats)
	{
		//color and name dont change during game
		background.color = UIColorDB.GetColor(pStats.Color);
		//todo: sortingOrder is not applied -> set it manually
		background.GetComponent<Canvas>().sortingOrder = -1;
		name.text = pStats.Name;

		//register for stats change
		pStats.SetOnStatsChange(OnStatsChanged);
		//invoke first change manually
		OnStatsChanged(pStats);
	}

	private void OnStatsChanged(PlayerStats pStats)
	{
		kills.text = pStats.Kills.ToString();
		deaths.text = pStats.Deaths.ToString();
	}
}
