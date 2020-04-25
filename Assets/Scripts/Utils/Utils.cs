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

	public static EDirection GetOppositeDirection(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return EDirection.Down;
			case EDirection.Right:
				return EDirection.Left;
			case EDirection.Down:
				return EDirection.Up;
			case EDirection.Left:
				return EDirection.Right;
		}
		return EDirection.None;
	}

	/// <summary>
	/// 50/50 coin toss
	/// </summary>
	internal static bool TossCoin()
	{
		return UnityEngine.Random.Range(0, 2) == 1;
	}

	/// <summary>
	/// Return direction rotated by 90 clockwise
	/// </summary>
	public static EDirection GetOrthogonalDirection(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return EDirection.Right;
			case EDirection.Right:
				return EDirection.Down;
			case EDirection.Down:
				return EDirection.Left;
			case EDirection.Left:
				return EDirection.Up;
		}
		return EDirection.None;
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

	internal static void DebugDrawCross(Vector3 pPosition, Color pColor)
	{
		const float length = 0.1f;
		const float duration = 0.5f;
		Debug.DrawLine(pPosition + Vector3.left * length, pPosition + Vector3.right * length, pColor, duration);
		Debug.DrawLine(pPosition + Vector3.down * length, pPosition + Vector3.up * length, pColor, duration);
	}

	internal static bool IsSameSign(float pNum1, float pNum2, bool pIgnoreZero)
	{
		if(pIgnoreZero && (IsNumEqual(pNum1, 0) || IsNumEqual(pNum2, 0)))
			return true;

		return Mathf.Sign(pNum1) == Mathf.Sign(pNum2);
	}

	internal static bool IsNumEqual(float pNumber, int pValue)
	{
		return Mathf.Abs(pNumber - pValue) < float.Epsilon;
	}
}
