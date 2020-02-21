using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIColorDB
{
	internal static Color GetColor(EPlayerColor pColor)
	{
		switch(pColor)
		{
			case EPlayerColor.Blue:
				return Color.blue;
			case EPlayerColor.Red:
				return Color.red;
			case EPlayerColor.Yellow:
				return Color.yellow;
			case EPlayerColor.Green:
				return Color.green;
			case EPlayerColor.Pink:
				return Color.magenta;
		}
		Debug.LogError("Wrong color requested: " + pColor);
		return Color.black;
	}
}
