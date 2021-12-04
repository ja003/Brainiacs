using AStarSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AiMovement : AiController
{
	public AiMovement(Player pPlayer) : base(pPlayer)
	{
		//seems to assign player.transform.position instead of playerPosition?
		//seems better without it
		//moveTarget = playerPosition;

		Debug.Log($"{player} move init");
		if(AstarAdapter.isInited)
		{
			astar = new Astar(AstarAdapter.grid);
			IsInited = true;
		}
		else
		{
			AstarAdapter.OnInited += () =>
			{
				astar = new Astar(AstarAdapter.grid);
				IsInited = true;
			};
		}
	}

	public bool IsInited;

	public const float DIST_CHECK_TOLERANCE = 0.1f;

	float MIN_RECALCULATE_PATH_FREQUENCY = 0.1f;
	float lastRecalculatePathTime;

	const float MOVE_PAUSE = 0.5f;
	float lastMovePauseTime; //todo: use this to force movement pause

	public const float PATH_STEP = Player.COLLIDER_SIZE / 2;

	bool isLogEnabled = false;

	Vector2 lastPos;

	float idleDuration;

	public Player TargetedPlayer;
	Vector2 moveTarget;

	Vector2 lastPathTarget;
	MovePath path = new MovePath();
	PathNode currentPathNode;

	int pathCalcId;
	public Astar astar;

	public void Update()
	{
		if(!isActive)
			return;

		if(Vector2.Distance(lastPos, player.Position2D) < float.Epsilon)
			idleDuration += Time.deltaTime;
		else
			idleDuration = 0;

		lastPos = player.Position2D;

		Utils.DebugDrawCross(moveTarget, Color.yellow);
		if(player.debug_DrawPath && path != null)
			Utils.DebugDrawPath(path.GetNodePositions(), Color.yellow);

		//Utils.DebugDrawCross(playerPosition, Color.red);
		//Utils.DebugDrawCross(player.transform.position, Color.blue);

		if(Time.time < lastMovePauseTime + MOVE_PAUSE)
			return;

		CheckPathProgress();

		//try recalculate path
		if(lastRecalculatePathTime + MIN_RECALCULATE_PATH_FREQUENCY < Time.time)
		{
			if(astar != null && player.isActiveAndEnabled)
				player.StartCoroutine(RecalculatePath());
		}
		else
		{
			UpdateMove();
		}
	}

	private void CheckPathProgress()
	{
		//check path progress
		if(IsCloseToCurrentPathNode())
		{
			//mark node as visited and select the next one
			currentPathNode.visited = true;
			if(path.IsNodeAtStraightPathToTarget(currentPathNode))
			{
				player.ai.Shoot.OnReachedStraightPathToTargetNode();
			}

			//check if is close to final target
			if(IsCloseToMoveTarget())
			{
				//if is close, try re-evaluate, target might have changed
				//brain.EvaluateGoals();
				//Debug.LogError("TODO: implement");//probably not needed


				//if is close even after re-evaluation, dont move
				if(IsCloseToMoveTarget())
				{
					//if player is targeted, look at him
					if(TargetedPlayer != null)
					{
						EDirection dirToPlayer = GetDirectionTo(TargetedPlayer.ColliderCenter);
						player.Movement.SetDirection(dirToPlayer);
					}
					if(path.IsCompleted())
					{
						player.Movement.Stop();
						currentPathNode = null;
						path.Clear(); //otherwise he gets stuck
						player.ai.Shoot.OnReachedTarget();
						return;
					}

					//brain.shoot.OnReachedTarget();
					//return;
				}
			}

			currentPathNode = path.GetFirstUnvisitedNode();
		}

		if(IsCloseToCurrentPathNode())
			CheckPathProgress();
	}

	public bool IsCloseTo(Vector2 pPosition)
	{
		float dist = Vector2.Distance(playerPosition, pPosition);
		return dist < DIST_CHECK_TOLERANCE;
	}

	private bool IsCloseToMoveTarget()
	{
		if(currentPathNode == null) return false;
		return IsCloseTo(moveTarget);
	}

	public void Stop()
	{
		player.Movement.Stop();

		if(playerPosition.x > 600)
			Debug.LogError("invalid position");

		if(player.isActiveAndEnabled)
			SetTarget(playerPosition);
	}

	public void SetTarget(Vector2 pTarget)
	{
		if(isLogEnabled)
			Debug.Log("SetTarget " + pTarget);

		if(astar == null)
			return;

		//pTarget might be on unwalkable position.
		//try to move it
		bool res = pathFinder.MoveToWalkablePos(ref pTarget, pTarget + Vector2.right, astar);
		if(!res)
			Debug.LogError($"SetTarget {pTarget} is unreachable");

		moveTarget = pTarget;
		if(astar != null && player.isActiveAndEnabled && !IsCloseTo(moveTarget) && aiBT.IsActive)
			player.StartCoroutine(RecalculatePath());
	}

	/// <summary>
	/// Ai cant change direction too soon after weapon use (looks weird).
	/// Mostly because of flamethrower visual effect.
	/// </summary>
	private bool CanChangeDirection()
	{
		const float min_change_dir_after_weapon_use_delay = 0.2f;
		//use LastUseStartTime! otherwise 0-cadency weapons (DaVinci, Flamethrower) 
		//returns always false when used
		return player.WeaponController.ActiveWeapon.LastUseStartTime + min_change_dir_after_weapon_use_delay < Time.time;
	}

	private bool IsCloseToCurrentPathNode()
	{
		if(currentPathNode == null)
			return false;

		return IsCloseTo(currentPathNode.point);
	}

	private IEnumerator RecalculatePath()
	{
		if(astar == null)
			goto end;

		pathCalcId++;
		//pathFinder.StopCalculation();

		lastRecalculatePathTime = Time.time;

		bool isIdleForTooLong = idleDuration > 0.5f;
		//if(isIdleForTooLong)
		//	Debug.Log("Idle for too long");

		//can be used for debug purpose as well
		bool forceRecalcPath = isIdleForTooLong;
		float newTargetDiff = Vector2.Distance(lastPathTarget, moveTarget);

		//check if player got off the path (eg. pushed out)
		bool isPlayerOffPath = false;
		if(currentPathNode?.Previous != null)
		{
			Vector2 previousToCurrentDirection = currentPathNode.point - currentPathNode.Previous.point;
			float playerDistFromLine = Utils.GetDistanceFromLineInDirection(
				playerPosition, currentPathNode.Previous.point, previousToCurrentDirection);
			isPlayerOffPath = playerDistFromLine > 2 * DIST_CHECK_TOLERANCE;
		}

		if(
			//path.IsValid() && 
			!forceRecalcPath && newTargetDiff < PATH_STEP / 2 && !isPlayerOffPath)
		{
			if(path != null)
				Utils.DebugDrawPath(path.GetNodePositions(), Color.blue);
			goto end;
		}
		else
		{
			//Debug.Log($"RecalculatePath - {pathCalcId} | {player.InitInfo.Number} | {Time.frameCount}");

			if(isLogEnabled)
			{
				Debug.Log($"RecalculatePath - {newTargetDiff}");
			}

			if(isIdleForTooLong)
				idleDuration = 0;

			astar.StopCalculation();
			player.StartCoroutine(pathFinder.GetPathAsync(playerPosition, moveTarget, astar));
			while(pathFinder.IsSearching)
				yield return new WaitForEndOfFrame();

			path = pathFinder.Path;

			if(path == null || !path.IsValid())
				goto end;

			OnPathRecalculated();
			CheckPathProgress();
			//moveTarget = path.First(); //NO!!
			lastPathTarget = moveTarget;
			//player.Movement.Stop(); //lagging
		}

	end: UpdateMove();
	}

	/// <summary>
	/// Path is calculated asynchronously. If it takes too long it is possible that 
	/// player as already located somewhere else than the start of the path.
	/// Check first X nodes if player is between some of them and mark them as visited.
	/// </summary>
	private void OnPathRecalculated()
	{
		if(path == null)
		{
			currentPathNode = null;
			return;
		}

		int maxCheckCount = Mathf.Min(3, path.nodes.Count);
		for(int i = 0; i < maxCheckCount; i++)
		{
			PathNode node = path.nodes[i];
			bool isCloseToNode = IsCloseTo(node.point);
			bool isBetweenThisAndNextNode = node.Next != null &&
				Utils.GetDistanceFromLine(playerPosition, node.point, node.Next.point) < DIST_CHECK_TOLERANCE;

			if(isCloseToNode || isBetweenThisAndNextNode)
			{
				for(int j = i; j >= 0; j--)
				{
					path.nodes[j].visited = true;
					Utils.DebugDrawCross(path.nodes[j].point, Color.red, 1);
				}
				break;
			}
		}

		currentPathNode = path.GetFirstUnvisitedNode();
	}

	private void UpdateMove()
	{
		if(currentPathNode == null || debug.PassiveAi)
		{
			player.Movement.Stop();
			return;
		}

		EDirection directionToTarget = GetDirectionTo(currentPathNode.point);
		if(directionToTarget == EDirection.Right)
		{
			//Debug.Log("");
		}

		if(directionToTarget != player.Movement.CurrentEDirection && !CanChangeDirection())
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
		//if(dir.magnitude < dist)
		if(IsCloseTo(pTarget))
		{
			direction = player.Movement.CurrentEDirection;
		}
		else if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
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
