using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiResults : ResultsController
{
	[SerializeField] private UiResultElement scoreFirst = null;
	[SerializeField] private UiResultElement scoreSecond = null;
	[SerializeField] private UiResultElement scoreThird = null;

	protected override void OnMainControllerAwaken()
	{
		if(debug.Result)
		{
			debug.SetResults();
		}

		SetResults();
	}

	private void SetResults()
	{
		PlayerScoreInfo result1 = brainiacs.GameResultInfo.GetResultInfo(1);
		scoreFirst.SetResult(result1);
		PlayerScoreInfo result2 = brainiacs.GameResultInfo.GetResultInfo(2);
		scoreSecond.SetResult(result2);
		PlayerScoreInfo result3 = brainiacs.GameResultInfo.GetResultInfo(3);
		scoreThird.SetResult(result3);
	}




}
