using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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


	internal static Vector3 GetVector3(Vector2 pDir)
	{
		return new Vector3(pDir.x, pDir.y, 0);
	}

	public static Vector2 GetVector2(EDirection pDirection)
	{
		Vector3 vec = GetVector3(pDirection);
		return new Vector2(vec.x, vec.y);
	}

	internal static Vector3 GetVector2(Vector3 pVector)
	{
		return new Vector2(pVector.x, pVector.y);
	}

	public static EDirection GetDirection(Vector2 pDirection)
	{
		if(pDirection.magnitude < 0.1f)
			return EDirection.None;

		float angle = Vector2.SignedAngle(Vector2.right, pDirection);
		if(angle >= -45 && angle < 45)
			return EDirection.Right;
		if(angle >= 45 && angle < 135)
			return EDirection.Up;
		if(angle >= -135 && angle < -45)
			return EDirection.Down;
		if(angle >= 135 || angle < -135)
			return EDirection.Left;

		Debug.LogError("Incorrect angle calculation for " + pDirection);
		return EDirection.None;
	}

	internal static void SetAlpha(Image pImage, float pValue)
	{
		pImage.color = new Color(
			pImage.color.r,
			pImage.color.g,
			pImage.color.b,
			pValue
			);
	}

	internal static bool IsWithinScreeen(Vector3 pPosition)
	{
		return
			pPosition.x < Screen.width &&
			pPosition.x > 0 &&
			pPosition.y > 0 &&
			pPosition.y < Screen.height;
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
				result = new Vector3(0, 0, 90);
				break;
			case EDirection.Right:
				result = Vector2.zero;
				break;
			case EDirection.Down:
				result = new Vector3(0, 0, -90);
				break;
			case EDirection.Left:
				result = new Vector3(0, 0, 180);
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

	internal static void DebugDrawCross(Vector2 pPosition, Color pColor, float pDuration = -1)
	{
		if(pDuration <= 0)
			pDuration = Time.deltaTime;

		const float length = 0.05f;
		//const float duration = 0.5f;
		Debug.DrawLine(pPosition + Vector2.left * length, pPosition + Vector2.right * length, pColor, pDuration);
		Debug.DrawLine(pPosition + Vector2.down * length, pPosition + Vector2.up * length, pColor, pDuration);
	}


	internal static void DebugDrawPath(List<Vector2> pPath, Color pColor, float pDuration = -1)
	{
		if(pDuration <= 0)
			pDuration = Time.deltaTime;

		for(int i = 0; i < pPath.Count - 1; i++)
		{
			float val = (float)i / pPath.Count;
			Color c = new Color(val, val, val);
			DebugDrawCross(pPath[i], c, pDuration);
			Debug.DrawLine(pPath[i], pPath[i + 1], pColor, pDuration);
		}
	}
		
	internal static void DebugDrawRect(Vector2 pTopLeft, Vector2 pBotRight, Color pColor, float pDuration = -1)
	{
		if(pDuration <= 0)
			pDuration = Time.deltaTime;

		Vector2 topRight = new Vector2(pBotRight.x, pTopLeft.y);
		Vector2 botLeft = new Vector2(pTopLeft.x, pBotRight.y);
		List<Vector2> boxPath = new List<Vector2>()
		{
			topRight, pBotRight, botLeft, pTopLeft, topRight
		};
		DebugDrawPath(boxPath, pColor, pDuration);
	}


	internal static void DebugDrawBox(Vector2 pCenter, Vector2 pSize, Color pColor, float pDuration = -1)
	{
		if(pDuration <= 0)
			pDuration = Time.deltaTime;

		Vector2 topRight = pCenter + pSize / 2;
		Vector2 botRight = topRight + Vector2.down * pSize.y;
		Vector2 botLeft = topRight - pSize;
		Vector2 topLeft = topRight + Vector2.left * pSize.x;
		List<Vector2> boxPath = new List<Vector2>()
		{
			topRight, botRight, botLeft, topLeft, topRight
		};
		DebugDrawPath(boxPath, pColor, pDuration);
	}

	internal static bool IsSameSign(float pNum1, float pNum2, bool pIgnoreZero)
	{
		if(pIgnoreZero && (Equals(pNum1, 0) || Equals(pNum2, 0)))
			return true;

		return Mathf.Sign(pNum1) == Mathf.Sign(pNum2);
	}

	internal static bool Equals(float pValue1, float pValue2, float pTolerance = -1)
	{
		if(pTolerance < 0)
			pTolerance = float.Epsilon;
		return Mathf.Abs(pValue1 - pValue2) < pTolerance;
	}

	internal static bool Equals(float pValue1, int pValue2, float pTolerance = -1)
	{
		return Equals(pValue1, (float)pValue2, pTolerance);
	}

	/// <summary>
	/// List<Vector2>.Contains doesnt seem to work correctly, probably due to float error
	/// </summary>
	internal static bool ContainsPoint(List<Vector2> pList, Vector2 pPoint, float pTolerance = 0.1f)
	{
		//start search from the last element => in this scope usually more effective
		for(int i = pList.Count - 1; i >=0; i--)
		{
			Vector2 p = pList[i];
			if(Vector2.Distance(p, pPoint) < pTolerance)
				return true;
		}
		return false;
	}


	public static Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 direction, Vector2 point)
	{
		direction.Normalize();
		Vector2 lhs = point - origin;

		float dotP = Vector2.Dot(lhs, direction);
		return origin + direction * dotP;
	}

	public static float GetDistanceFromLine(Vector2 pPoint, Vector2 pLineStart, Vector2 pLineDirection)
	{
		Vector2 nearestPoint = FindNearestPointOnLine(pLineStart, pLineDirection, pPoint);
		float dist = Vector2.Distance(pPoint, nearestPoint);
		return dist;
	}

	public static int PositiveModulo(int x, int m)
	{
		return (x % m + m) % m;
	}
}
