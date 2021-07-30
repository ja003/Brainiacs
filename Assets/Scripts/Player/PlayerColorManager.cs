using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorManager : BrainiacsBehaviour
{
	[SerializeField] private Color playerColor_Red = Color.red;
	[SerializeField] private Color playerColor_Green = Color.green;
	[SerializeField] private Color playerColor_Blue = Color.blue;
	[SerializeField] private Color playerColor_Yellow = Color.yellow;
	[SerializeField] private Color playerColor_Pink = Color.magenta;
	[SerializeField] private Color playerColor_Gray = Color.gray;


	[SerializeField] private Color lightColor_Red = Color.red;
	[SerializeField] private Color lightColor_Green = Color.green;
	[SerializeField] private Color lightColor_Blue = Color.blue;
	[SerializeField] private Color lightColor_Yellow = Color.yellow;
	[SerializeField] private Color lightColor_Pink = Color.magenta;


	public Color GetColor(EPlayerColor pColor, float pAlpha = 1)
	{
		Color color = Color.white;
		switch(pColor)
		{
			case EPlayerColor.None:
				color = Color.white;
				break;
			case EPlayerColor.Blue:
				color = playerColor_Blue;
				break;
			case EPlayerColor.Red:
				color = playerColor_Red;
				break;
			case EPlayerColor.Yellow:
				color = playerColor_Yellow;
				break;
			case EPlayerColor.Green:
				color = playerColor_Green;
				break;
			case EPlayerColor.Pink:
				color = playerColor_Pink;
				break;
			case EPlayerColor.Gray:
				color = playerColor_Gray;
				break;
			default:
				Debug.LogError("Color not found");
				break;
		}

		return new Color(color.r, color.g, color.b, pAlpha);
	}

	public Color GetLightColor(EPlayerColor pColor)
	{
		Color color = Color.white;
		switch(pColor)
		{
			case EPlayerColor.None:
				color = Color.white;
				break;
			case EPlayerColor.Blue:
				color = lightColor_Blue;
				break;
			case EPlayerColor.Red:
				color = lightColor_Red;
				break;
			case EPlayerColor.Yellow:
				color = lightColor_Yellow;
				break;
			case EPlayerColor.Green:
				color = lightColor_Green;
				break;
			case EPlayerColor.Pink:
				color = lightColor_Pink;
				break;
			default:
				Debug.LogError("Color not found");
				break;
		}

		return color;
	}
}
