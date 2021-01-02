using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenu : Tutorial
{
	MainMenu mainMenu => MainMenu.Instance;

	protected override void OnActivated()
	{
	}

	protected override void OnCompleted()
	{
		mainMenu.Tutorial.OnCompleted(tutorialPart);
	}

	protected override void SetBgEnabled(bool pValue)
	{
		mainMenu.Tutorial.Background.enabled = pValue;
	}
}
