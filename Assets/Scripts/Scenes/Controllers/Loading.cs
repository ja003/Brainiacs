﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : GameBehaviour
{
	[SerializeField]
	private Slider slider;

	protected override void Awake()
	{
		base.Awake();
		Brainiacs.InstantiateSingleton();

		Brainiacs.SelfInitGame = false;

		DoInTime(LoadGame, 1);
	}
	
	private void LoadGame()
	{
		Debug.Log("LoadGame");
		StartCoroutine(Brainiacs.Instance.Scenes.LoadSceneAsync(
			EScene.Game, slider,
			() => StartCoroutine(Countdown())));
	}

	private IEnumerator Countdown()
	{
		Debug.Log("Countdown");

		for(int i = 1; i < 4; i++)
		{
			Debug.Log("Countdown: " + i);
			yield return new WaitForSeconds(1); 
		}
		StartGame();
	}

	private void StartGame()
	{
		Debug.Log("StartGame");

		Game.Instance.Activate();
		Brainiacs.Instance.Scenes.UnloadScene(EScene.Loading);
	}

	public void TestLoad()
	{
		LoadGame();
	}

	public void TestStart()
	{
		StartGame();

	}
}
