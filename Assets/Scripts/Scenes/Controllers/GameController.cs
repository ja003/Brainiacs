using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController : ControllerBehaviour
{
	protected Game game => Game.Instance;

	protected override BrainiacsBehaviour GetMainController()
	{
		return game;
	}

}
