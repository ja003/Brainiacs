using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGame : Tutorial
{
	Game game => Game.Instance;

	protected override void OnActivated()
	{
		game.GameTime.SetPause(true);
	}

	protected override void OnCompleted()
	{
		game.Tutorial.OnCompleted(tutorialPart);

		if(next == null)
			game.GameTime.SetPause(false);

	}

	protected override void SetBgEnabled(bool pValue)
	{
		game.Tutorial.Background.enabled = pValue;
	}
}
