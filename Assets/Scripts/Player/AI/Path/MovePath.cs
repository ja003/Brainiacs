using AStarSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Path for AI player to walk through.
/// Path nodes remember if they were visited.
/// </summary>
public class MovePath
{
	//First = start, Last = goal
	public List<PathNode> nodes = new List<PathNode>();
	float stepSize;

	public MovePath()
	{
		Clear();
	}

	public MovePath(List<Vector2> pNodes, float pStepSize)
	{
		Clear();
		stepSize = pStepSize;
		foreach(var node in pNodes)
		{
			nodes.Add(new PathNode(node));
		}
		for(int i = 0; i < nodes.Count; i++)
		{
			if(i > 0)
				nodes[i].Previous = nodes[i - 1];
			if(i < nodes.Count - 2)
				nodes[i].Next = nodes[i + 1];
		}
	}

	/// <summary>
	/// Resets path nodes
	/// </summary>
	public void Clear()
	{
		nodes.Clear();
	}

	/// <summary>
	/// Has the last node been visited?
	/// If the path is empty => false
	/// </summary>
	public bool IsCompleted()
	{
		return nodes.Count == 0 ? false : nodes.Last().visited;
	}

	public List<Vector2> GetNodePositions()
	{
		List<Vector2> nodesPos = new List<Vector2>();
		foreach(var n in nodes)
		{
			nodesPos.Add(n.point);
		}
		return nodesPos;
	}

	public PathNode GetStart()
	{
		return nodes.First();
	}

	/// <summary>
	/// Returns the first unvisited node.
	/// NOTE: not effective at all, but the path is expected to be recalculated very often
	/// so it shouldn't have very big impact on performance.
	/// </summary>
	internal PathNode GetFirstUnvisitedNode()
	{
		foreach(var node in nodes)
		{
			if(!node.visited)
				return node;
		}
		return null;
	}

	public override string ToString()
	{
		string s = "path: " + Environment.NewLine;
		for(int i = 0; i < nodes.Count; i++)
		{
			PathNode n = nodes[i];
			s += i + " - " + n + Environment.NewLine;
		}
		return base.ToString();
	}


	internal bool IsNodeAtStraightPathToTarget(PathNode pNode)
	{
		if(nodes.Count < 3)
			return true;
		Vector2 dirToNode = pNode.point - nodes.Last().point;
		Vector2 dir2ndToLast = pNode.point - nodes[nodes.Count - 2].point;
		Vector2 dir3rdToLast = pNode.point - nodes[nodes.Count - 3].point;

		//angle between the pNode and 2nd and 3rd has to be 0 on straight path
		float angle2to3 = Vector2.Angle(dir3rdToLast, dir2ndToLast);
		if(angle2to3 > 1)
			return false;

		//angle to the last node might bi higher since the last path segment 
		//doesnt have to be othogonal
		float angle = Vector2.Angle(dirToNode, dir2ndToLast);
		const int straight_line_angle_tolerance = 10;
		//Debug.Log($"angle = {angle}");

		return angle < straight_line_angle_tolerance;
	}

	/// <summary>
	/// Returns number of nodes * step size.
	/// Not actual length, just approximation (last nodes distance may be lower than step size)
	/// </summary>
	internal float GetLength()
	{
		return nodes.Count * stepSize;
	}

	internal bool IsValid()
	{
		return nodes.Count > 1;
	}

}
