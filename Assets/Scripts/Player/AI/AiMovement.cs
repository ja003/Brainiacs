using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMovement : AiController
{
	public AiMovement(PlayerAiBrain pBrain, Player pPlayer) : base(pBrain, pPlayer)
	{
	}


	public void Update()
	{
		if(IsCloseToTarget())
		{
			//if is close, try re-evaluate, target might have changed
			brain.EvaluateGoals();

			//if is close even after re-evaluation, dont move
			if(IsCloseToTarget())
			{
				//if player is targeted, look at him
				if(TargetedPlayer != null) {
					EDirection dirToPlayer = GetDirectionTo(TargetedPlayer.transform.position);
					player.Movement.SetDirection(dirToPlayer);
				}
				player.Movement.Stop();

				brain.shoot.OnReachedTarget();
				return;
			}
		}

		EDirection moveDirectionToTarget = GetDirectionTo(moveTarget);
		player.Movement.SetMove(moveDirectionToTarget);

		Utils.DebugDrawCross(moveTarget, Color.yellow);
	}

	private bool IsCloseToTarget()
	{
		const float max_dist_to_target = 0.1f;
		return Vector3.Distance(playerPosition, moveTarget) < max_dist_to_target;
	}

	public Player TargetedPlayer;
	Vector3 moveTarget;

	public void SetTarget(Vector3 pTarget)
	{
		moveTarget = pTarget;
		//Debug.Log("SetTarget " + pTarget);
	}

	private EDirection GetDirectionTo(Vector3 pTarget)
	{
		Vector3 dir = pTarget - playerPosition;
		EDirection direction;
		if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
		{
			direction = dir.x > 0 ? EDirection.Right : EDirection.Left;
		}
		else
		{
			direction = dir.y > 0 ? EDirection.Up: EDirection.Down;
		}
		return direction;
	}
}
