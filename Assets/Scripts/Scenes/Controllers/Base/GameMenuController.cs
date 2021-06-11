using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Menu in game scene
/// </summary>
public abstract class GameMenuController : MenuController
{
	protected Game game => Game.Instance;

	protected override BrainiacsBehaviour GetMainController()
	{
		return game;
	}

}
