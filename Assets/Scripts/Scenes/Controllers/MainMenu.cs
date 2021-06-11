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
	[SerializeField] private Button btnJoinGame = null;
	[SerializeField] private Button btnSettings = null;
	[SerializeField] private Button btnQuit = null;

	[SerializeField] int debug_InitBgAnim = MENU_ANIM_POSITION_MAIN;

	[SerializeField] public UIGameSetupMain SetupMain = null;
	//[SerializeField] public UIGameSetup GameSetup;
	[SerializeField] public MainMenuPhoton Photon;
	[SerializeField] public UIInputKeySelector InputKeySelector;

	[SerializeField] public TutorialMenuController Tutorial;

	[SerializeField] public UIGameSetupSearch SetupSearch = null;

	[SerializeField] public UIInfoMessenger InfoMessenger;

	protected override void Awake()
	{
		//if(!debug.release)
		{
			if(debug_InitBgAnim != MENU_ANIM_POSITION_MAIN)
				Debug.LogError("Default menu position changed [test?]");
			SetMenuAnimPosition(debug_InitBgAnim);
		}
		btnStartGame.onClick.AddListener(OnBtnStartGame);
		btnJoinGame.onClick.AddListener(OnBtnJoinGame);
		btnSettings.onClick.AddListener(OnBtnSettings);
		btnQuit.onClick.AddListener(OnBtnQuit);

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
		SetupMain.SetActive(true, true);
		SetupSearch.SetActive(false);
	}

	private void OnBtnJoinGame()
	{
		StartMenuAnim(MENU_ANIM_POSITION_SETUP);
		SetupSearch.SetActive(true);
		SetupMain.SetActive(false);
	}

	private void OnBtnSettings()
	{
		StartMenuAnim(MENU_ANIM_POSITION_SETTINGS);
	}

	public void OnBtnBack()
	{
		StartMenuAnim(MENU_ANIM_POSITION_MAIN);
	}

	private void OnBtnQuit()
	{
		Application.Quit();
	}

	int menuAnimId;

	private void StartMenuAnim(int pTargetMenuAnimPosition)
	{
		LeanTween.cancel(menuAnimId);
		float animTime = debug.InstaAnimation ? 0 : ANIM_TIME;
		menuAnimId = UpdateValue(currentMenuAnimPosition, pTargetMenuAnimPosition, animTime, SetMenuAnimPosition);
	}

	private void SetMenuAnimPosition(float pValue)
	{
		//Debug.Log("SetMenuAnimPosition: " + pValue);
		currentMenuAnimPosition = pValue;
		animator.SetFloat("position", pValue);
	}

}
