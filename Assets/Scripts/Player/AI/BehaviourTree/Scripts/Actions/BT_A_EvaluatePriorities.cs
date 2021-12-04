using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System;

public class BT_A_EvaluatePriorities : ActionNode
{

	protected override void OnStart()
	{
		List<Tuple<int, EAiGoal>> priorityOfGoals = new List<Tuple<int, EAiGoal>>();

		//get priority of controllers
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(1, EAiGoal.None));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(context.Shoot.GetPriority(), EAiGoal.Shoot));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(context.Evade.GetPriority(), EAiGoal.Evade));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(context.Item.GetPriority(), EAiGoal.PickupItem));
		priorityOfGoals.Add(new Tuple<int, EAiGoal>(context.debug.GetPriority(), EAiGoal.Debug));

		//pick the highest priority controller
		priorityOfGoals.Sort((b, a) => a.Item1.CompareTo(b.Item1)); //sort descending

		//set current goal
		blackboard.Goal = priorityOfGoals[0].Item2;
	}

	protected override void OnStop()
	{
	}

	protected override State OnUpdate()
	{
		return State.Success;
	}
}
