using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Palette", menuName = "UI color palette")]
public class UiPalette : ScriptableObject
{
	[SerializeField] List<Color> colors = new List<Color>() { Color.white };

	public Color GetColor(int pColorIndex)
	{
		if(colors.Count > pColorIndex)
			return colors[pColorIndex];

		Debug.LogError($"Color {pColorIndex} not specified");
		return Color.white;
	}
}
