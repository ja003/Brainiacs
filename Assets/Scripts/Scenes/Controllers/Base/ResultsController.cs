using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResultsController : ControllerBehaviour
{

	protected Results results => Results.Instance;

	protected override BrainiacsBehaviour GetMainController()
	{
		return results;
	}
}