using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiMovement : AiController
{
	public AiMovement(PlayerAiBrain pBrain, Player pPlayer) : base(pBrain, pPlayer)
	{
		//seems to assign player.transform.position instead of playerPosition?
		//seems better without it
		//moveTarget = playerPosition;
	}

	float MIN_RECALCULATE_PATH_FREQUENCY = 0.1f;
	float lastRecalculatePathTime;

	const float MOVE_PAUSE = 0.5f;
	float lastMovePauseTime; //todo: use this to force movement pause

	public const float PATH_STEP = Player.COLLIDER_SIZE / 2;


	public void Update()
	{
		Utils.DebugDrawCross(moveTarget, Color.yellow);
		Utils.DebugDrawPath(path.GetNodePositions(), Color.yellow);

		//Utils.DebugDrawCross(playerPosition, Color.red);
		//Utils.DebugDrawCross(player.transform.position, Color.blue);


		if(Time.time < lastMovePauseTime + MOVE_PAUSE)
			return;

		//check path progress
		if(IsCloseToCurrentPathNode())
		{
			//mark node as visited and select the next one
			currentPathNode.visited = true;
			if(path.IsNodeAtStraightPathToTarget(currentPathNode))
			{
				brain.shoot.OnReachedStraightPathToTargetNode();
			}

			//check if is close to final target
			if(IsCloseToTarget())
			{
				//if is close, try re-evaluate, target might have changed
				brain.EvaluateGoals();

				//if is close even after re-evaluation, dont move
				if(IsCloseToTarget())
				{
					//if player is targeted, look at him
					if(TargetedPlayer != null)
					{
						EDirection dirToPlayer = GetDirectionTo(TargetedPlayer.Position);
						player.Movement.SetDirection(dirToPlayer);
					}
					if(path.IsCompleted())
					{
						player.Movement.Stop();
						currentPathNode = null;
						path.Clear(); //otherwise he gets stucked
						brain.shoot.OnReachedTarget();
						return;
					}

					//brain.shoot.OnReachedTarget();
					//return;
				}
			}

			currentPathNode = path.GetFirstUnvisitedNode();
		}

		

		//if(IsCloseToCurrentPathNode())
		//{
		//	//mark node as visited and select the next one
		//	currentPathNode.visited = true;
		//	if(path.IsNodeAtStraightPathToTarget(currentPathNode))
		//	{
		//		brain.shoot.OnReachedStraightPathToTargetNode();
		//	}

		//	currentPathNode = path.GetFirstUnvisitedNode();
		//}

		//try recalculate path
		if(lastRecalculatePathTime + MIN_RECALCULATE_PATH_FREQUENCY < Time.time)
		{
			RecalculatePath();
			//if(currentPathNode != null)
			//{
			//	EDirection directionToTarget = GetDirectionTo(currentPathNode.point);
			//	if(directionToTarget != player.Movement.CurrentDirection && !CanChangeDirection())
			//	{
			//		Debug.Log("Cant change direction so soon after weapon use");
			//		return;
			//	}
			//	else
			//	{
			//		player.Movement.SetMove(directionToTarget);
			//	}
			//}
		}
		else
		{
			UpdateMove();
		}
	}

	private bool IsCloseToTarget()
	{
		if(currentPathNode == null) return false;

		const float max_dist_to_target = 0.1f;
		return Vector2.Distance(playerPosition, moveTarget) < max_dist_to_target;
	}

	public Player TargetedPlayer;
	Vector2 moveTarget;

	public void SetTarget(Vector2 pTarget)
	{
		moveTarget = pTarget;
		RecalculatePath();
		//Debug.Log("SetTarget " + pTarget);
	}

	/// <summary>
	/// Ai cant change direction too soon after weapon use (looks weird).
	/// Mostly because of flamethrower visual effect.
	/// </summary>
	private bool CanChangeDirection()
	{
		const float min_change_dir_after_weapon_use_delay = 0.2f;
		return player.WeaponController.ActiveWeapon.LastUseTime + min_change_dir_after_weapon_use_delay < Time.time;
	}

	private bool IsCloseToCurrentPathNode()
	{
		if(currentPathNode == null)
			return false;

		return Vector2.Distance(playerPosition, currentPathNode.point) < 0.1f;
	}

	Vector2 lastPathTarget;
	MovePath path = new MovePath();
	PathNode currentPathNode;

	private void RecalculatePath()
	{
		lastRecalculatePathTime = Time.time;


		bool forceRecalcPath = false;
		float newTargetDiff = Vector2.Distance(lastPathTarget, moveTarget);

		//check if player got off the path (eg. pushed out)
		float playerDistToNode = currentPathNode == null ? 
			0 : Vector2.Distance(playerPosition, currentPathNode.point);
		bool isPlayerOffPath = playerDistToNode > PATH_STEP * 1.2f;
			//|| currentPathNode == null; //path is null => is off path

		//Debug.Log("isPlayerOffPath = " + isPlayerOffPath);

		if(!forceRecalcPath && newTargetDiff < PATH_STEP / 2 && !isPlayerOffPath)
		{
			Utils.DebugDrawPath(path.GetNodePositions(), Color.blue);
			return;
		}
		else
		{
			path = PathFinder.GetPath(playerPosition, moveTarget, PATH_STEP, true);
			currentPathNode = path.GetFirstUnvisitedNode();
			Utils.DebugDrawPath(path.GetNodePositions(), Color.green, MIN_RECALCULATE_PATH_FREQUENCY);

			//moveTarget = path.First(); //NO!!
			lastPathTarget = moveTarget;
			//player.Movement.Stop(); //lagging
		}

		UpdateMove();
	}

	private void UpdateMove()
	{
		if(currentPathNode == null)
		{
			player.Movement.Stop();
			return;
		}

		EDirection directionToTarget = GetDirectionTo(currentPathNode.point);
		if(directionToTarget != player.Movement.CurrentDirection && !CanChangeDirection())
		{
			//Debug.Log("Cant change direction so soon after weapon use");
			//player.Movement.Stop(); //causes lagging on a straight path
			return;
		}
		else
		{
			player.Movement.SetMove(directionToTarget);
		}
	}

	private EDirection GetDirectionTo(Vector2 pTarget)
	{
		Vector2 dir = pTarget - playerPosition;
		EDirection direction;
		if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
		{
			direction = dir.x > 0 ? EDirection.Right : EDirection.Left;
		}
		else
		{
			direction = dir.y > 0 ? EDirection.Up : EDirection.Down;
		}
		return direction;
	}
}
