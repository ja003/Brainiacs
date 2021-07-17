using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiResults : ResultsController
{
	[SerializeField] private UiResultElement resultsElPrefab;

	protected override void OnMainControllerAwaken()
	{
		if(debug.Result)
		{
			debug.SetResults();
		}
		resultsElPrefab.gameObject.SetActive(false);

		SetResults();
	}

	private void SetResults()
	{
		for(int i = 1; i <= brainiacs.GameResultInfo.PlayerResults.Count; i++)
		{
			PlayerScoreInfo scoreInfo = brainiacs.GameResultInfo.GetResultInfo(i);
			UiResultElement newScoreEl = Instantiate(resultsElPrefab, transform);
			newScoreEl.SetResult(i, scoreInfo);
			newScoreEl.gameObject.SetActive(true);
		}
		
	}




}
