using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button for choosing player input key
/// </summary>
public class UiInputKey : UiBehaviour
{
	[SerializeField] Text keyText;
	[SerializeField] TextMeshProUGUI keyTextPro;
	UIInputKeySelector selector => MainMenu.Instance.InputKeySelector;

	public KeyCode Key;
	public Action OnKeySet;

	protected override void Awake()
	{
		base.Awake();
		button.onClick.AddListener(() => StartCoroutine(selector.GetKeyPressed(SetKey)));
	}

	public void SetKey(KeyCode pKey)
	{
		//Debug.Log(gameObject.name + " set " + pKey);
		Key = pKey;
		if(keyText != null)
			keyText.text = GetKeyName(pKey);
		else if(keyTextPro != null)
			keyTextPro.text = GetKeyName(pKey);
		else
			Debug.LogError("no text field set");

		OnKeySet?.Invoke();
	}

	private static string GetKeyName(KeyCode pKey)
	{
		switch(pKey)
		{
			case KeyCode.UpArrow:
				return "↑";
			case KeyCode.RightArrow:
				return "→";
			case KeyCode.DownArrow:
				return "↓";
			case KeyCode.LeftArrow:
				return "←";
		}

		string name = pKey.ToString();
		//numpad numbers
		name = name.Replace("Keypad", "");
		//F - numbers
		name = name.Replace("Alpha", "");
		//Right/Left Shift/Alt/Ctrl
		name = name.Replace("Right", "R - ");
		name = name.Replace("Left", "L - ");
		name = name.Replace("Control", "Ctrl");

		return name;
	}
}
