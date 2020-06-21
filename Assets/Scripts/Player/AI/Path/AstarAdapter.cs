﻿using AStarSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

using SVector2 = System.Numerics.Vector2;

public static class AstarAdapter
{
	static Astar astar;
	static float stepScale => Node.NODE_SIZE / stepSize;
	static float stepSize;

	//all coordinates have to be positive in Astar
	static Vector2 positionShift;

	static Vector2 playerSize;

	static List<List<Node>> grid;

	public static void Init(float pStepSize, Vector2 pTopLeft, Vector2 pBotRight)
	{
		playerSize = Game.Instance.PlayerManager.PLAYER_SIZE;

		stepSize = pStepSize;
		grid = new List<List<Node>>();

		Vector2 botLeft = new Vector2(pTopLeft.x, pBotRight.y);
		Vector2 topRight = new Vector2(pBotRight.x, pTopLeft.y);
		positionShift = -botLeft;

		for(float x = botLeft.x; x < topRight.x; x += pStepSize)
		{
			grid.Add(new List<Node>());

			for(float y = botLeft.y; y < topRight.y; y += pStepSize)
			{
				Vector2 nodePos = new Vector2(x, y);
				bool isWalkable = !PathFinderController.OverlapsWithMapObject(nodePos);

				//Utils.DebugDrawCross(nodePos, isWalkable ? Color.green : Color.red, 1000);

				SVector2 nodePosScaled = GetScaledVector(nodePos);

				Node node = new Node(nodePosScaled, isWalkable);
				grid.Last().Add(node);
			}
		}

		astar = new Astar(grid);
		Debug_DrawGrid();
	}

	public static void Debug_DrawGrid()
	{
		foreach(var col in grid)
		{
			foreach(var row in col)
			{
				Vector2 nodePos = GetScaledVector(row.Center);

				Utils.DebugDrawCross(nodePos, row.Walkable ? Color.green : Color.red);
			}
		}
	}

	private static SVector2 GetScaledVector(Vector2 pVector)
	{
		pVector += positionShift;
		return new SVector2(pVector.x * stepScale, pVector.y * stepScale);
	}

	private static Vector2 GetScaledVector(SVector2 pVector)
	{
		Vector2 scaledVector = new Vector2(pVector.X / stepScale, pVector.Y / stepScale);
		return scaledVector - positionShift;
	}


	//public static async Task<MovePath> GetPathAsync(Vector2 pFrom, Vector2 pTo)
	////public static async IEnumerator<MovePath> GetPathAsync(Vector2 pFrom, Vector2 pTo)
	//{
	//	Debug.Log("GetPathAsync ->");
	//	//yield return new WaitForEndOfFrame();
	//	await Task.Delay(1);
	//	Debug.Log("<- GetPathAsync");

	//	return GetPath(pFrom, pTo);
	//}

	public static async Task<MovePath> GetPath(Vector2 pFrom, Vector2 pTo)
	{
		//handled better below
		//if(PathFinderController.OverlapsWithMapObject(pTo))
		//	return new MovePath();

		SVector2 start = GetScaledVector(pFrom);
		SVector2 end = GetScaledVector(pTo);

		//if pTo is unreachable, try to find the closest node and navigate to it
		if(!astar.IsWalkable(end))
		{
			//Debug.Log($"End {end} is unreachable");
			SVector2? closest = astar.GetClosestWalkable(end, start);
			if(closest == null)
			{
				//Debug.LogError($"GetPath: Cant reach {end}");
				return new MovePath();
			}
			end = (SVector2)closest;
		}
		await Task.Delay(1);

		var pathStack = await astar.FindPathAsync(start, end);

		List<Vector2> pathNodes = new List<Vector2>();
		pathNodes.Add(pFrom);

		//skip the first node - otherwise the path starts with sharp turn.
		pathStack?.Pop();

		bool isPathValid = pathStack != null && pathStack.Count > 0;

		//try to join the next node with simple node
		Vector2 joinTarget = !isPathValid ? pTo : GetScaledVector(pathStack.Peek().Center);
		Vector2? joinPoint = PathFinderController.GetJoinPoint(pFrom, joinTarget);
		if(joinPoint == null && !isPathValid)
		{
			return new MovePath();
		}

		if(joinPoint != null)
			pathNodes.Add((Vector2)joinPoint);

		//add path nodes
		while(isPathValid && pathStack.Count > 0)
		{
			pathNodes.Add(GetScaledVector(pathStack.Pop().Center));
		}
		pathNodes.Add(pTo);

		MovePath path = new MovePath(pathNodes, stepSize);
		return path;
	}
}
