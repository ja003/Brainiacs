using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIApplyPaletteColor : MonoBehaviour
{
	[SerializeField] int colorIndex;

	public void Apply(UiPalette pPalette)
	{
		Color color = pPalette.GetColor(colorIndex);
		Debug.Log($"{gameObject.name} apply {color}");

		if(GetComponent<Image>() != null)
			GetComponent<Image>().color = color;
		else if(GetComponent<Text>() != null)
			GetComponent<Text>().color = color;
		else if(GetComponent<TextMeshProUGUI>() != null)
			GetComponent<TextMeshProUGUI>().color = color;
		else
			Debug.LogWarning($"{gameObject.name} doesnt have any applicable color component");
	}
}
