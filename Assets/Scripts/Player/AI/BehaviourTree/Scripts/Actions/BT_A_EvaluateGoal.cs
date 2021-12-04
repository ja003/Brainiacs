using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class BT_A_EvaluateGoal : ActionNode
{
	float startTime;
	bool isInvalidGoal;
	protected override void OnStart()
	{
		startTime = Time.time;
		isInvalidGoal = false;

		//Debug.Log("goal = " + blackboard.Goal);

		if(!context.BTController.isInited)
			return;

		switch(blackboard.Goal)
		{
			case EAiGoal.None:
				//context.Movement.Stop();
				//isInvalidGoal = true;
				break;
			case EAiGoal.Shoot:

				context.Movement.TargetedPlayer = context.Shoot.targetedPlayer;
				//aiMovement.SetTarget(shoot.moveTarget);
				context.Movement.SetTarget(context.Shoot.GetTarget());

				break;
			case EAiGoal.Evade:
				context.Movement.SetTarget(context.Evade.GetTarget());
				break;
			case EAiGoal.PickupItem:
				context.Movement.SetTarget(context.Item.GetTarget());
				break;

			case EAiGoal.Debug:
				context.Movement.SetTarget(context.debug.GetTarget());
				break;
			default:
				isInvalidGoal = true;
				break;

		}
	}

	protected override void OnStop()
	{
	}

	protected override State OnUpdate()
	{
		if(isInvalidGoal)
			return State.Failure;

		context.Movement.Update();
		context.Shoot.Update();
		context.Item.Update();

		return Time.time - startTime < Blackboard.EVALUATE_FREQUENCY ? State.Running : State.Success;
	}
}
