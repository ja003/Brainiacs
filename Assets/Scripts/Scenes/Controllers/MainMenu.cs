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



	protected override void Awake()
	{
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
		Debug.Log("OnBtnStartGame");
		UpdateValue(currentMenuAnimPosition, MENU_ANIM_POSITION_SETUP, ANIM_TIME, SetMenuAnimPosition);
	}

	public void OnBtnBack()
	{
		Debug.Log("OnBtnBack");
		UpdateValue(currentMenuAnimPosition, MENU_ANIM_POSITION_MAIN, ANIM_TIME, SetMenuAnimPosition);
	}

	private void SetMenuAnimPosition(float pValue)
	{
		currentMenuAnimPosition = pValue;
		animator.SetFloat("position", pValue);
	}


	//private void OnBtnStartGame()
	//{
	//	brainiacs.TestSetGameInitInfo();
	//	Brainiacs.Instance.Scenes.LoadScene(EScene.Loading);
	//}

	//private void TestSetGameInitInfo()
	//{
	//	GameInitInfo = new GameInitInfo();
	//	GameInitInfo.players.Add(new PlayerInitInfo(EHero.Tesla));
	//	GameInitInfo.players.Add(new PlayerInitInfo(EHero.Currie));

	//	GameInitInfo.Mode = EGameMode.Time;
	//	GameInitInfo.Time = 5;
	//}

}
