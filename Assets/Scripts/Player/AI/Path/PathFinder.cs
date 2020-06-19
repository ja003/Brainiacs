using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Simple path finder without any big optimization.
/// Paths are expected not to be very long so performance shouldnt be an issue.
/// todo: 
/// 1) make interface for PathFinder, try different implementations
/// 2) try making step size adjustable 
///  - longer steps at beginning
///  - prevent too frequent turns (diagonal move)
/// 3) maybe further path analysis for cutting
/// </summary>
public static class PathFinder
{
	/// <summary>
	/// Returns path between given points on grid starting at pFrom and having given cell size
	/// </summary>
	/// <returns></returns>
	public static MovePath GetPath(Vector2 pFrom, Vector2 pTo, float pStepSize, bool pDebug = false)
	{
		//Debug.Log("GetPath");

		const bool test_path = false;
		if(test_path)
		{
			pFrom = Vector2.zero;
			pTo = pFrom + new Vector2(1, 1);
		}


		List<Vector2> path = new List<Vector2>();
		//pFrom was commented out...not sure why, seems to work ok.
		//if not add to the path, add it to rejected
		path.Add(pFrom);

		int iter = 0;
		Vector2 lastPoint = pFrom;
		//visited points leading to dead-end path
		List<Vector2> rejected = new List<Vector2>();

		//if goal is too close to an obstacle it would be impossible to reach it in some cases
		// -> be less restrictive (NOTE: not valid solution)
		float maxDistToGoal = pStepSize * 1.5f;
		while(Vector2.Distance(lastPoint, pTo) > maxDistToGoal)
		{
			if(pDebug)
				Utils.DebugDrawCross(lastPoint, Color.red);

			List<Vector2> neighbours = GetNeighbours(lastPoint, pStepSize, path.Concat(rejected).ToList());
			//no neighbour found => it is dead-end => reject last point
			if(neighbours.Count == 0)
			{
				if(path.Count == 0)
				{
					Debug.Log("Path not found 1!");
					path.Add(pFrom);
					break;
				}

				rejected.Add(lastPoint);
				path.RemoveAt(path.Count - 1);

				if(path.Count == 0)
				{
					Debug.Log("Path not found 2!");
					path.Add(pFrom);
					break;
				}

				lastPoint = path.Last();
				continue;
			}
			//alwasy select the closest point to the goal
			neighbours.Sort((a, b) => Vector2.Distance(a, pTo).CompareTo(Vector2.Distance(b, pTo)));
			lastPoint = neighbours.First();
			path.Add(lastPoint);

			//check last X nodes if there is a possibility to reach the last node
			//via shorter path => cut it.
			// - 5 seems to be too low
			const int max_node_check = 7;
			TryCutThePath(path, max_node_check, pStepSize, rejected, pDebug);

			//try to avoid too much "zig-zag-ing"
			TryStraightenThePath(path, pStepSize, pDebug);

			iter++;
			const int max_iterations = 500;
			if(iter > max_iterations)
			{
				Debug.LogError("Too many path find iterations");
				break;
			}
		}

		float distToLast = Vector2.Distance(path.Last(), pTo);
		if(distToLast > 0.1f)
		{
			//add the last shorter step - select randomly
			//note: character is expected to be able to reach them, no physics testing
			Vector2 p1 = new Vector2(path.Last().x, pTo.y);
			Vector2 p2 = new Vector2(pTo.x, path.Last().y);
			path.Add(Utils.TossCoin() ? p1 : p2);
		}

		path.Add(pTo);
		return new MovePath(path, pStepSize);
	}

	/// <summary>
	/// Check if the last 4 nodes are in zig-zag shape.
	/// If so, try to replace one node with new one making the pPath more straight.
	/// X			X - X
	/// |				|
	/// X - X	=>		X
	///		|			|
	///		X			X
	/// </summary>
	private static void TryStraightenThePath(List<Vector2> pPath, float pStepSize, bool pDebug)
	{
		if(pPath.Count < 4)
			return;
		if(!IsZigZag(pPath.GetRange(pPath.Count - 4, 4), pStepSize))
			return;

		Vector2 p1 = pPath[pPath.Count - 4];
		Vector2 p2 = pPath[pPath.Count - 3];
		Vector2 p3 = pPath[pPath.Count - 2];
		Vector2 p4 = pPath[pPath.Count - 1];

		Utils.DebugDrawPath(new List<Vector2>()
		{
			p1,p2,p3,p4
		}, Color.red);

		Vector2 newPoint = p1 + (p2 - p1) * 2;
		if(!OverlapsWithMapObject(newPoint))
		{
			pPath.RemoveAt(pPath.Count - 2);
			pPath.Insert(pPath.Count - 1, newPoint);
			return;
		}
		newPoint = p4 + (p3 - p4) * 2;
		if(!OverlapsWithMapObject(newPoint))
		{
			pPath.RemoveAt(pPath.Count - 3);
			pPath.Insert(pPath.Count - 2, newPoint);
			return;
		}

	}

	/// <summary>
	/// Checks if the first 4 nodes of pPath are zig-zag shaped
	/// X
	/// |
	/// X - X
	///		|
	///		X
	/// </summary>
	private static bool IsZigZag(List<Vector2> pPath, float pStep)
	{
		if(pPath.Count < 4)
			return false;

		Vector2 dir1 = pPath[1] - pPath[0];
		Vector2 dir2 = pPath[2] - pPath[1];
		Vector2 dir3 = pPath[3] - pPath[2];

		//both angles have to be 90°
		float angle1 = Mathf.Abs(Vector2.Angle(dir1, dir2));
		float angle2 = Mathf.Abs(Vector2.Angle(dir2, dir3));

		const float angle_tolerance = 1;
		const int ortho_angle = 90;
		bool isOrthogonal1 = Utils.IsNumEqual(angle1, ortho_angle, angle_tolerance);
		bool isOrthogonal2 = Utils.IsNumEqual(angle2, ortho_angle, angle_tolerance);
		if(isOrthogonal1 && isOrthogonal2)
		{
			//prevent case:
			//X   X
			//|	  |
			//X - X
			float dist = Vector2.Distance(pPath[0], pPath[3]);
			//in zig-zag is distance between first and last definitely higher
			return dist > 2 * pStep;
		}
		return false;
	}

	/// <summary>
	/// Check if some of the last pMaxNodeChecks nodes can be reached from 
	/// one of previous nodes more effectively.
	/// If so, cut the path
	/// </summary>
	private static void TryCutThePath(List<Vector2> pPath, int pMaxNodeChecks, float pStep,
		List<Vector2> pRejected, bool pDebug = false)
	{
		if(pPath.Count < 4)
			return;

		Vector2 target = pPath.Last();
		for(int i = pPath.Count - pMaxNodeChecks; i < pPath.Count - 3; i++)
		{
			if(i < 0) continue;
			if(IsNeighbours(pPath[i], target, pStep))
			{
				for(int j = pPath.Count - 2; j > i; j--)
				{
					pRejected.Add(pPath[j]);
					pPath.RemoveAt(j);
				}
				//pPath.RemoveRange(i + 1, count);
				//pRejected.AddRange(
				if(pDebug)
				{
					//path cut from X to Y
					Debug.DrawLine(pPath.Last(), pPath[pPath.Count - 2], Color.magenta, 0.5f);
				}

				return;
			}
		}
	}

	private static bool IsNeighbours(Vector2 pNode1, Vector2 pNode2, float pStep)
	{
		return Math.Abs(Vector2.Distance(pNode1, pNode2) - pStep) < 0.1f;
	}

	/// <summary>
	/// Returns point in 4-neighbourhood from the pCenter in pStep distance which are not
	/// in pIgnorePoints and dont collide with any map object (using Player collider size)
	/// </summary>
	private static List<Vector2> GetNeighbours(Vector2 pCenter, float pStep,
		List<Vector2> pIgnorePoints, bool pDeadlockFlag = false)
	{
		List<Vector2> neighbours = new List<Vector2>();

		Vector2 top = pCenter + Vector2.up * pStep;
		Vector2 right = pCenter + Vector2.right * pStep;
		Vector2 down = pCenter + Vector2.down * pStep;
		Vector2 left = pCenter + Vector2.left * pStep;

		//if(!Utils.ContainsPoint(pIgnorePoints, top) && !OverlapsWithMapObject(top))
		//	neighbours.Add(top);
		//if(!Utils.ContainsPoint(pIgnorePoints, right) && !OverlapsWithMapObject(right))
		//	neighbours.Add(right);
		//if(!Utils.ContainsPoint(pIgnorePoints, down) && !OverlapsWithMapObject(down))
		//	neighbours.Add(down);
		//if(!Utils.ContainsPoint(pIgnorePoints, left) && !OverlapsWithMapObject(left))
		//	neighbours.Add(left);

		//try with pMaxStepMultiply...probably wont help
		//pMaxStepMultiply = Mathf.Max(1, pMaxStepMultiply);
		//for(int i = 1; i <= pMaxStepMultiply; i++)
		//{
		//	Vector2 top = pCenter + Vector2.up * pStep * i;
		//	bool added = TryAddPointToCollection(pCenter, pIgnorePoints, neighbours, top);
		//	if(!added)
		//		break;
		//}

		TryAddPointToCollection(pCenter, pIgnorePoints, neighbours, top);
		TryAddPointToCollection(pCenter, pIgnorePoints, neighbours, right);
		TryAddPointToCollection(pCenter, pIgnorePoints, neighbours, down);
		TryAddPointToCollection(pCenter, pIgnorePoints, neighbours, left);

		if(neighbours.Count == 0 && !pDeadlockFlag)
		{
			return GetNeighbours(pCenter, 2 * pStep, pIgnorePoints, true);
		}

		return neighbours;
	}

	/// <summary>
	/// Adds the pPoint to the pCollection if it does not overlap with
	/// any map object and is not in pIgnorePoints.
	/// Returns true if added.
	/// </summary>
	private static bool TryAddPointToCollection(Vector2 pCenter, List<Vector2> pIgnorePoints, List<Vector2> pCollection, Vector2 pPoint)
	{
		//note: IsReachable method does not work well when player is touching 
		//a map object (always unreachable)
		//if(!Utils.ContainsPoint(pIgnorePoints, pPoint) && IsReachable(pCenter, pPoint))
		if(!Utils.ContainsPoint(pIgnorePoints, pPoint) && !OverlapsWithMapObject(pPoint))
		{
			pCollection.Add(pPoint);
			return true;
		}
		return false;
	}

	private static bool IsReachable(Vector2 pFrom, Vector2 pTo)
	{
		//the path finder is tailored to the Player -> use its collider
		float collWidthHalf = Player.ColliderSize.x / 2;
		float collHeightHalf = Player.ColliderSize.y / 2;
		Vector2 topLeft = new Vector2(Mathf.Min(pFrom.x, pTo.x) - collWidthHalf, Mathf.Max(pFrom.y, pTo.y) + collHeightHalf);
		Vector2 botRight = new Vector2(Mathf.Max(pFrom.x, pTo.x) + collWidthHalf, Mathf.Min(pFrom.y, pTo.y) - collHeightHalf);
		Collider2D overlaps = Physics2D.OverlapArea(topLeft, botRight, Layers.UnwalkableObject);
		if(overlaps)
		{
			Utils.DebugDrawRect(topLeft, botRight, Color.yellow);
		}
		return !overlaps;
	}


	/// <summary>
	/// The path finder is tailored to the Player -> use its collider
	/// </summary>
	private static bool OverlapsWithMapObject(Vector2 pPoint)
	{
		//const float player_size = 0.2f; //todo: connect to real value?
		Collider2D overlaps = Physics2D.OverlapBox(pPoint, Player.ColliderSize, 0, Layers.UnwalkableObject);
		if(overlaps)
		{
			//Debug.Log("OverlapsWithMapObject " + overlaps.gameObject.name);
			//Utils.DebugDrawBox(pPoint, Player.ColliderSize, Color.yellow);
		}

		return overlaps;
		//return Physics2D.OverlapCircle(pPoint, Player.COLLIDER_SIZE, mapObject); //not enough
		//return Physics2D.OverlapPoint(pPoint, mapObject);
	}
}
