using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : BrainiacsBehaviour
{
	[SerializeField]
	private Slider slider;

	[SerializeField] int countdown;

	[SerializeField] Image background;

	MapConfig currentMapConfig;

	const float BEEP_DELAY = 1;

	protected override void Awake()
	{	
		Brainiacs.SelfInitGame = false;

		EMap map = brainiacs.GameInitInfo.Map;
		if(map == EMap.None)
		{
			Debug.LogError("Map not set");
			map = EMap.Steampunk;
		}

		currentMapConfig = brainiacs.MapManager.GetMapConfig(map);

		SetBackground(0);
		DoInTime(LoadGame, BEEP_DELAY);


		base.Awake(); //always call base.event() at the end
	}

	private void SetBackground(int pIndex)
	{
		switch(pIndex)
		{
			case 0:
				SoundController.PlaySound(ESound.Loading_Beep_1, audioSource);
				background.sprite = currentMapConfig.Loading1;
				break;
			case 1:
				SoundController.PlaySound(ESound.Loading_Beep_2, audioSource);
				background.sprite = currentMapConfig.Loading2;
				break;
			case 2:
				SoundController.PlaySound(ESound.Loading_Beep_3, audioSource);
				background.sprite = currentMapConfig.Loading3;
				break;
			case 3:
				SoundController.PlaySound(ESound.Loading_Beep_4, audioSource);
				background.sprite = currentMapConfig.Loading4;
				break;
		}
	}


	//TODO: implement photon scene loading

	private void LoadGame()
	{
		Debug.Log("LoadGame");
		
		StartCoroutine(Countdown());
		//StartCoroutine(Brainiacs.Instance.Scenes.LoadScene(
		//	EScene.Game, slider,
		//	() => StartCoroutine(Countdown())));
	}

	private IEnumerator Countdown()
	{
		Debug.Log("Countdown");

		for(int i = 1; i <= countdown; i++)
		{
			Debug.Log("Countdown: " + i);
			SetBackground(i);
			yield return new WaitForSeconds(BEEP_DELAY);
		}
		StartGame();
	}

	private void StartGame()
	{
		Debug.Log("StartGame");

		//Game.Instance.Activate();
		Brainiacs.Instance.Scenes.LoadScene(EScene.Game);
	}

	/*
	public void TestLoad()
	{
		LoadGame();
	}

	public void TestStart()
	{
		StartGame();

	}*/
}
