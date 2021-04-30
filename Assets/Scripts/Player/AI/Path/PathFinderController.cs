using AStarSharp;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using SVector2 = System.Numerics.Vector2;

public class PathFinderController : GameBehaviour
{
	static Vector2 playerSize;

	Map activeMap => Game.Instance.Map.ActiveMap;

	public void Init(float pStepSize, Vector2 pTopLeft, Vector2 pBotRight)
	{
		playerSize = Game.Instance.PlayerManager.PLAYER_SIZE;
		Game.Instance.StartCoroutine(AstarAdapter.Init(pStepSize, pTopLeft, pBotRight));
	}

	//public void Init(float pStepSize, Vector2 pTopLeft, Vector2 pBotRight)
	//{
	//    AstarAdapter.Init(pStepSize, pTopLeft, pBotRight);
	//}


	//  public MovePath GetPath(Vector2 pFrom, Vector2 pTo)
	//  {
	////Debug.Log($"GetPath {pFrom}-{pTo}");

	//if(!activeMap.IsWithinMap(pFrom) || !activeMap.IsWithinMap(pTo))
	//{
	//	return new MovePath();
	//}

	//return AstarAdapter.GetPath(pFrom, pTo);
	//  }


	public MovePath Path;
	public bool IsSearching;

	public IEnumerator GetPathAsync(Vector2 pFrom, Vector2 pTo, Astar pAstar)
	{
		//Debug.Log($"GetPath {pFrom}-{pTo}");
		Path = new MovePath();
		IsSearching = true;

		//if(!activeMap.IsWithinMap(pFrom) || !activeMap.IsWithinMap(pTo))
		//	goto end;

		if(!activeMap.IsWithinMap(pFrom))
			pFrom = activeMap.GetPositionWithinMap(pFrom);
		if(!activeMap.IsWithinMap(pTo))
			pTo = activeMap.GetPositionWithinMap(pTo);

		//while(!isInited)
		//	await Task.Delay(100);
		if(!AstarAdapter.isInited)
			goto end;

		//handled better below
		//if(PathFinderController.OverlapsWithMapObject(pTo))
		//	return new MovePath();

		SVector2 start = AstarAdapter.GetScaledVector(pFrom);
		SVector2 end = AstarAdapter.GetScaledVector(pTo);

		//if pTo is unreachable, try to find the closest node and navigate to it
		MoveToWalkablePos(ref end, start, pAstar);

		yield return new WaitForEndOfFrame();
		StartCoroutine(pAstar.FindPathAsync(start, end));
		while(pAstar.IsSearching)
		{
			yield return new WaitForEndOfFrame();
		}
		var pathStack = pAstar.Path;

		List<Vector2> pathNodes = new List<Vector2>();
		pathNodes.Add(pFrom);

		//skip the first node - otherwise the path starts with sharp turn.
		if(pathStack != null && pathStack.Count > 0)
			pathStack.Pop();

		bool isPathValid = pathStack != null && pathStack.Count > 0;

		//try to join the next node with simple node
		Vector2 joinTarget = !isPathValid ?
			AstarAdapter.GetScaledVector(end) :
			AstarAdapter.GetScaledVector(pathStack.Peek().Center);
		Vector2? joinPoint = GetJoinPoint(pFrom, joinTarget);
		if(joinPoint == null && !isPathValid)
			goto end;

		if(joinPoint != null)
			pathNodes.Add((Vector2)joinPoint);

		//add path nodes
		while(isPathValid && pathStack.Count > 0)
		{
			pathNodes.Add(AstarAdapter.GetScaledVector(pathStack.Pop().Center));
		}
		pathNodes.Add(AstarAdapter.GetScaledVector(end));

		Path = new MovePath(pathNodes, AstarAdapter.stepSize);
		end: IsSearching = false;
		//return path;
	}

	/// <summary>
	/// Scales the input param positions and executes MoveToWalkablePos.
	/// Suited for external call.
	/// </summary>
	public bool MoveToWalkablePos(ref Vector2 pPos, Vector2 pRefPos, Astar pAstar)
	{
		SVector2 scaledPos = AstarAdapter.GetScaledVector(pPos);
		bool res = MoveToWalkablePos(ref scaledPos, AstarAdapter.GetScaledVector(pRefPos), pAstar);
		pPos = AstarAdapter.GetScaledVector(scaledPos);
		return res;
	}

	/// <summary>
	/// Moves pPos to walkable position closest to the pRefPos.
	/// </summary>
	/// <returns>True if any walkable position is found</returns>
	private bool MoveToWalkablePos(ref SVector2 pPos, SVector2 pRefPos, Astar pAstar)
	{
		if(!pAstar.IsWalkable(pPos))
		{
			//Debug.Log($"End {end} is unreachable");
			SVector2? closest = pAstar.GetClosestWalkable(pPos, pRefPos);
			//Debug.Log($"closest  = {closest }");
			if(closest == null)
				return false;
			else
				pPos = (SVector2)closest;
		}
		return true;
	}


	/// <summary>
	/// The path finder is tailored to the Player -> use its collider + AI movement tollerance
	/// </summary>
	public static bool OverlapsWithMapObject(Vector2 pPoint)
	{
		//add DIST_CHECK_TOLERANCE so the player doesnt get stack when close to some obstackle
		Vector2 playerOverlapSize = playerSize + Vector2.one * AiMovement.DIST_CHECK_TOLERANCE / 2;
		Collider2D overlaps = Physics2D.OverlapBox(pPoint, playerOverlapSize, 0, Layers.UnwalkableObject);
		if(overlaps)
		{
			//Debug.Log("OverlapsWithMapObject " + overlaps.gameObject.name);
			//Utils.DebugDrawBox(pPoint, playerSize, Color.yellow, 1);
		}

		return overlaps;
		//return Physics2D.OverlapCircle(pPoint, Player.COLLIDER_SIZE, mapObject); //not enough
		//return Physics2D.OverlapPoint(pPoint, mapObject);
	}

	/// <summary>
	/// Returns point connecting pFrom and pTo in the simplest way.
	/// X		X - P
	/// |			|
	/// P - Y		Y
	/// The point cant overlap with any map object.
	/// </summary>
	public static Vector2? GetJoinPoint(Vector2 pFrom, Vector2 pTo)
	{
		Vector2 p1 = new Vector2(pFrom.x, pTo.y);
		Vector2 p2 = new Vector2(pTo.x, pFrom.y);
		if(!OverlapsWithMapObject(p1))
			return p1;
		else if(!OverlapsWithMapObject(p2))
			return p2;

		return null;
	}

	public MovePath GetPath(Vector2 pFrom, Vector2 pTo)
	{
		throw new System.NotImplementedException();
	}

	//public void StopCalculation()
	//{
	//	AstarAdapter.StopCalculation();
	//}
}

//public interface IPathFinder
//{
//	// void Init(float pStepSize, Vector2 pTopLeft, Vector2 pBotRight);

//	MovePath GetPath(Vector2 pFrom, Vector2 pTo);

//	Task<MovePath> GetPathAsync(Vector2 pFrom, Vector2 pTo, Astar astar);

//	//void StopCalculation();
//}