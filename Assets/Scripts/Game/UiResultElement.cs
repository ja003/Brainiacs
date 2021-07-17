using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UiResultElement : BrainiacsBehaviour
{
	[SerializeField] private Text rank;
	[SerializeField] private Image heroPortrait = null;
	[SerializeField] private Text playerName;
	[SerializeField] private Text kills;
	[SerializeField] private Text deaths;

	public void SetResult(int pRank, PlayerScoreInfo pInfo)
	{
		rank.text = pRank + ".";
		if(pInfo == null)
		{
			gameObject.SetActive(false);
			return;
		}
		kills.text = pInfo.Kills.ToString();
		deaths.text = pInfo.Deaths.ToString();
		heroPortrait.sprite = brainiacs.HeroManager.GetHeroConfig(pInfo.Hero).Portrait;
		playerName.text = Utils.GetFormattedText(pInfo.Name, pInfo.Color);
	}
}
