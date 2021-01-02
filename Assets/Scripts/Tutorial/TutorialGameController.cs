using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGameController : GameController
{
	[SerializeField] TutorialGame tut1;
	[SerializeField] public Image Background;

	protected override void OnMainControllerAwaken()
	{
		game.OnGameStarted += OnGameStarted;
		//game.PlayerManager.OnAllPlayersAdded.AddAction(OnGameStarted);
	}

	internal bool IsTutorialActive()
	{
		for(int i = 0; i < transform.childCount; i++)
		{
			if(transform.GetChild(i).gameObject.activeSelf)
				return true;
		}
		return false;
	}

	internal void OnCompleted(ETutorial pTutorialPart)
	{
		switch(pTutorialPart)
		{
			case ETutorial.None:
				break;
			case ETutorial.Game_Controls:
				PlayerPrefs.SetInt(pTutorialPart.ToString(), 1);
				break;
			default:
				break;
		}
	}

	private void OnGameStarted()
	{
		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
		if(debug.SkipTutorial)
			return;

		bool isGameControlsDone = PlayerPrefs.GetInt(ETutorial.Game_Controls.ToString()) > 0;

		if(!isGameControlsDone || debug.ForceTutorial)
		{
			tut1.Activate();
		}
	}
}
