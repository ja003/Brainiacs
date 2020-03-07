using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results : BrainiacsBehaviour
{
	[SerializeField] private ResultScore scoreFirst;
	[SerializeField] private ResultScore scoreSecond;
	[SerializeField] private ResultScore scoreThird;

	protected override void Awake()
	{
		base.Awake();

		SetScore();
	}

	private void SetScore()
	{
		scoreFirst.SetResult(brainiacs.GameResultInfo.GetResultInfo(1));
		scoreSecond.SetResult(brainiacs.GameResultInfo.GetResultInfo(2));
		scoreThird.SetResult(brainiacs.GameResultInfo.GetResultInfo(3));
	}

	public void TestClose()
	{
		brainiacs.Scenes.LoadScene(EScene.MainMenu);
	}
}
