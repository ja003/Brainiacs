using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDebug : AiGoalController
{
	public AiDebug(PlayerAiBrain pBrain, Player pPlayer) : base(pBrain, pPlayer, EAiGoal.Debug)
	{
		
	}

	public override void Evaluate()
	{
	}

	public override int GetPriority()
	{
		return CDebug.Instance.AiDebugMove ? 666 : 0;
	}

	public override Vector2 GetTarget()
	{
		return game.GameDebug.AiMoveTarget.position;
	}
}
