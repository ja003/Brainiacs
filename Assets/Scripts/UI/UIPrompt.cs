using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPrompt : MenuController
{
    [SerializeField] Text message;
	[SerializeField] Text note;
	[SerializeField] Button btnOk;
	[SerializeField] Button btnCancel;

	//game/menu prompt.
	//kinda hacky solution but we dont use prompt in other scenes
	[SerializeField] bool isGame;

	protected override void OnMainControllerAwaken()
	{
		btnCancel.onClick.AddListener(Hide);

		if(Time.timeSinceLevelLoad < 1)
			Hide();
	}


	public void Show(string pText, string pNote, bool pEnableCancel, UnityAction pOnOk)
	{
		SetActive(true);

		btnCancel.gameObject.SetActive(pEnableCancel);

		message.text = pText;
		note.text = pNote;
		note.gameObject.SetActive(pNote.Length > 0);
		btnOk.onClick.RemoveAllListeners();
		btnOk.onClick.AddListener(pOnOk);
		btnOk.onClick.AddListener(Hide);
	}

	private void Hide()
	{
		SetActive(false);
	}

	protected override BrainiacsBehaviour GetMainController()
	{
		if(isGame)
			return Game.Instance;
		else
			return MainMenu.Instance;
	}

	
}
