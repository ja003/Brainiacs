using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UiResultElement : BrainiacsBehaviour
{
	[SerializeField] private TextMeshProUGUI score = null;
	[SerializeField] private Image heroPortrait = null;
	[SerializeField] private TextMeshProUGUI playerName = null;

	public void SetResult(PlayerScoreInfo pInfo)
	{
		if(pInfo == null)
		{
			gameObject.SetActive(false);
			return;
		}
		score.text = $"{pInfo.Kills} / {pInfo.Deaths}";
		heroPortrait.sprite = brainiacs.HeroManager.GetHeroConfig(pInfo.Hero).Portrait;
		playerName.text = pInfo.Name;
	}
}
