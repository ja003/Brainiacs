using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIPaletteManager : EditorWindow
{
	UiPalette activePalette;

	[MenuItem("Tools/UIPaletteManager")]
	public static void ShowWindow()
	{
		GetWindow(typeof(UIPaletteManager));
	}

	private void Awake()
	{
		Debug.Log("Awake");
		Init();
	}

	private void Init()
	{
		//todo: load last activePalette
	}

	private void OnGUI()
	{
		activePalette = EditorGUILayout.ObjectField("Active palette", activePalette, typeof(UiPalette), false) as UiPalette;

		if(GUILayout.Button("Apply"))
		{
			Apply();
		}

		if(GUILayout.Button("Add script [UIApplyPaletteColor] to all Images"))
		{
			AddScriptToImages();
		}

		if(GUILayout.Button("Add script [UIApplyPaletteColor] to all Texts"))
		{
			AddScriptToTexts();
		}

		if(GUILayout.Button("Add script [UIApplyPaletteColor] to all TextMeshPros"))
		{
			AddScriptToTMPro();
		}
	}

	private void AddScriptToTexts()
	{
		Text[] objsInScene = FindObjectsOfType<Text>();
		for(int i = 0; i < objsInScene.Length; i++)
		{
			if(objsInScene[i].GetComponent<UIApplyPaletteColor>() == null)
			{
				objsInScene[i].gameObject.AddComponent<UIApplyPaletteColor>();
				Debug.Log($"Add UIApplyPaletteColor to {objsInScene[i].gameObject.name}");
			}
		}
	}

	private void AddScriptToImages()
	{
		Image[] objsInScene = FindObjectsOfType<Image>();
		for(int i = 0; i < objsInScene.Length; i++)
		{
			if(objsInScene[i].GetComponent<UIApplyPaletteColor>() == null)
			{
				objsInScene[i].gameObject.AddComponent<UIApplyPaletteColor>();
				Debug.Log($"Add UIApplyPaletteColor to {objsInScene[i].gameObject.name}");
			}
		}
	}

	private void AddScriptToTMPro()
	{
		TextMeshProUGUI[] objsInScene = FindObjectsOfType<TextMeshProUGUI>();
		for(int i = 0; i < objsInScene.Length; i++)
		{
			if(objsInScene[i].GetComponent<UIApplyPaletteColor>() == null)
			{
				objsInScene[i].gameObject.AddComponent<UIApplyPaletteColor>();
				Debug.Log($"Add UIApplyPaletteColor to {objsInScene[i].gameObject.name}");
			}
		}
	}

	private void Apply()
	{
		UIApplyPaletteColor[] objsInScene = FindObjectsOfType<UIApplyPaletteColor>();
		for(int i = 0; i < objsInScene.Length; i++)
		{
			objsInScene[i].Apply(activePalette);
		}
		Repaint();
		EditorUtility.SetDirty(this);
	}

}
