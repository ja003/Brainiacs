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

	[SerializeField] private Button btnStartGame;
	[SerializeField] private Button btnSettings;
	[SerializeField] private Button btnQuit;

	[SerializeField] public UIGameSetup GameSetup;

	[SerializeField] int debug_InitBgAnim;

	protected override void Awake()
	{
		SetMenuAnimPosition(debug_InitBgAnim);

		btnStartGame.onClick.AddListener(OnBtnStartGame);
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
	
	public void OnBtnBack()
	{
		StartMenuAnim(MENU_ANIM_POSITION_MAIN);
	}

	private void StartMenuAnim(int pTargetMenuAnimPosition)
	{
		LeanTween.cancel(gameObject);
		UpdateValue(currentMenuAnimPosition, pTargetMenuAnimPosition, ANIM_TIME, SetMenuAnimPosition);
	}

	private void SetMenuAnimPosition(float pValue)
	{
		currentMenuAnimPosition = pValue;
		animator.SetFloat("position", pValue);
	}	
}
