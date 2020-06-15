using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : CSingleton<MainMenu>
{
	private const int ANIM_TIME = 1;
	private const int MENU_ANIM_POSITION_SETUP = 1;
	private const int MENU_ANIM_POSITION_MAIN = 0;
	private const int MENU_ANIM_POSITION_SETTINGS = -1;

	[SerializeField] private Button btnStartGame = null;
	[SerializeField] private Button btnSettings = null;
	[SerializeField] private Button btnQuit = null;

	[SerializeField] int debug_InitBgAnim = -1;

	[SerializeField] public UIGameSetup GameSetup;
	[SerializeField] public MainMenuPhoton Photon;

	protected override void Awake()
	{
		SetMenuAnimPosition(debug_InitBgAnim);

		btnStartGame.onClick.AddListener(OnBtnStartGame);
		btnSettings.onClick.AddListener(OnBtnSettings);
		//btnBack.onClick.AddListener(OnBtnBack);

		if(Brainiacs.SelfInitGame)
		{
			//todo: Activate from scene loading?
			DoInTime(Activate, 0.5f);
		}

		base.Awake(); //always call base.event() at the end
	}

	float currentMenuAnimPosition = 0;
	private void OnBtnStartGame()
	{
		StartMenuAnim(MENU_ANIM_POSITION_SETUP);
	}

	private void OnBtnSettings()
	{
		StartMenuAnim(MENU_ANIM_POSITION_SETTINGS);
	}

	public void OnBtnBack()
	{
		StartMenuAnim(MENU_ANIM_POSITION_MAIN);
	}

	int menuAnimId;

	private void StartMenuAnim(int pTargetMenuAnimPosition)
	{
		LeanTween.cancel(menuAnimId);
		menuAnimId = UpdateValue(currentMenuAnimPosition, pTargetMenuAnimPosition, ANIM_TIME, SetMenuAnimPosition);
	}

	private void SetMenuAnimPosition(float pValue)
	{
		currentMenuAnimPosition = pValue;
		animator.SetFloat("position", pValue);
	}

}
