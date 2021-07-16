using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenuController : MainMenuController
{
	[SerializeField] public Image Background;
	[SerializeField] Tutorial tut1;
	[SerializeField] Tutorial debug_tut1;



	protected override void OnMainControllerAwaken()
	{
		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
		if(debug.SkipTutorial)
			return;

		bool isMenuStartGameDone = PlayerPrefs.GetInt(ETutorial.Menu_StartGame.ToString()) > 0;

		if(!isMenuStartGameDone || debug.ForceTutorial)
		{
			Debug.Log("Menu tutorial");

			if(debug_tut1 != null)
				debug_tut1.Activate();
			else
				tut1.Activate();
		}
		else
		{
			Debug.Log("Menu tutorial already done");
		}
	}

	internal void OnCompleted(ETutorial pTutorialPart)
	{
		switch(pTutorialPart)
		{
			case ETutorial.None:
				break;
			case ETutorial.Menu_StartGame:
				PlayerPrefs.SetInt(pTutorialPart.ToString(), 1);
				break;
			default:
				break;
		}
	}
}

