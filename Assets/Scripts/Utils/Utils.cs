using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
	public static Vector3 GetVector3(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return Vector3.up;
			case EDirection.Right:
				return Vector3.right;
			case EDirection.Down:
				return Vector3.down;
			case EDirection.Left:
				return Vector3.left;
		}
		return Vector3.zero;
	}

	public static Vector2 GetVector2(EDirection pDirection)
	{
		Vector3 vec = GetVector3(pDirection);
		return new Vector2(vec.x, vec.y);
	}
}
