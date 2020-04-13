using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiResultElement : BrainiacsBehaviour
{
	[SerializeField]
	private TextMeshProUGUI score;

	[SerializeField]
	private Image heroPortrait;

	[SerializeField]
	private TextMeshProUGUI name;

	public void SetResult(PlayerScoreInfo pInfo)
	{
		if(pInfo == null)
		{
			gameObject.SetActive(false);
			return;
		}
		score.text = $"{pInfo.Kills} / {pInfo.Deaths}";
		heroPortrait.sprite = brainiacs.HeroManager.GetHeroConfig(pInfo.Hero).Portrait;
		name.text = pInfo.Name;
	}
}
