using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

	/// <summary>
	/// Returns Z rotation vector. Right = 0
	/// </summary>
	public static Vector3 GetRotation(EDirection pDirection, float pOffset = 0)
	{
		Vector3 result = Vector3.zero;
		switch(pDirection)
		{
			case EDirection.Up:
				result =  new Vector3(0, 0, 90);
				break;
			case EDirection.Right:
				result = Vector3.zero;
				break;
			case EDirection.Down:
				result = new Vector3(0, 0, -90);
				break;
			case EDirection.Left:
				result =  new Vector3(0, 0, 180);
				break;
		}
		result.z += pOffset;
		return result;
	}

	public static List<string> GetStrings(Type pType, int pExclude = -1)
	{
		List<string> values = Enum.GetNames(pType).OfType<string>().ToList();
		if(pExclude > -1)
		{
			values.RemoveAt(pExclude);
		}
		return values;
	}
}
