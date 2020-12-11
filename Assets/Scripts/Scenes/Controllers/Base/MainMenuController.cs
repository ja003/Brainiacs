using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainMenuController : ControllerBehaviour
{

	protected MainMenu mainMenu => MainMenu.Instance;

	protected override BrainiacsBehaviour GetMainController()
	{
		return mainMenu;
	}
}