using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI score;

	[SerializeField]
	private TextMeshProUGUI heroImage; //todo: change to Image

	[SerializeField]
	private TextMeshProUGUI name;

	public void SetResult(PlayerResultInfo pInfo)
	{
		if(pInfo == null)
		{
			gameObject.SetActive(false);
			return;
		}
		score.text = $"{pInfo.Kills} / {pInfo.LivesLeft}";
		heroImage.text = pInfo.Hero.ToString();
		name.text = pInfo.Name;
	}
}
