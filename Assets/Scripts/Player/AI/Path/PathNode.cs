using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Element of a MovePath
/// </summary>
public class PathNode
{
	public Vector2 point;
	public bool visited;

	public PathNode(Vector2 point)
	{
		this.point = point;
	}

	public override string ToString()
	{
		return $"[{point.x:0.0},{point.y:0.0}]({visited})";
	}
}