using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPaletteManager : EditorWindow
{
	UiPalette activePalette;
	[SerializeField]
	GameObject targetObject;

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

		targetObject = EditorGUILayout.ObjectField("Target object", targetObject, typeof(GameObject), true) as GameObject;

		if(GUILayout.Button("Apply"))
		{
			Apply();
		}

		if(GUILayout.Button("Add script [UIApplyPaletteColor] to all Images"))
		{
			AddScriptTo<Image>();
		}

		if(GUILayout.Button("Add script [UIApplyPaletteColor] to all Texts"))
		{
			AddScriptTo<Text>();
		}

		if(GUILayout.Button("Add script [UIApplyPaletteColor] to all TextMeshPros"))
		{
			AddScriptTo<TextMeshProUGUI>();
		}
	}

	private void AddScriptTo<T>() where T : UIBehaviour
	{
		T[] objsInScene = targetObject == null ?
			FindObjectsOfType<T>() :
			targetObject.GetComponentsInChildren<T>(true);

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
