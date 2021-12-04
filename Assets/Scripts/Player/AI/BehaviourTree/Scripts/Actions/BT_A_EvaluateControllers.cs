using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class BT_A_EvaluateControllers : ActionNode
{

	protected override void OnStart()
	{
		//evaluate all controllers

		context.Shoot.Evaluate();
		context.Evade.Evaluate();
		context.Item.Evaluate();
		context.debug.Evaluate();
	}

	protected override void OnStop()
	{
	}

	protected override State OnUpdate()
	{
		return State.Success;
	}
}
