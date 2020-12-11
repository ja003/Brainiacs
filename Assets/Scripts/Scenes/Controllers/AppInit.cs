using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppInit : BrainiacsBehaviour
{
	protected override void Awake()
	{
		Brainiacs.InstantiateSingleton();
		base.Awake();

		DoInTime(LoadMainMenu, 1);
	}

	private void LoadMainMenu()
	{
		brainiacs.Scenes.LoadScene(EScene.MainMenu);
	}
}
