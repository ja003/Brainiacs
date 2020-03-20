﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : CSingleton<Game>
{
	[SerializeField]
	Camera mainCamera;

	[SerializeField]
	public MapController MapController;

	[SerializeField]
	public PlayerManager PlayerManager;

	[SerializeField]
	public ProjectileManager ProjectileManager;

	[SerializeField]
	public UIPlayerStatusManager PlayerStatusManager;

	[SerializeField] public MobileInput MobileInput;

	[SerializeField] Button btnEnd;

	protected override void Awake()
	{		
		if(Brainiacs.SelfInitGame)
		{
			DoInTime(Activate, 0.5f);
		}
		btnEnd.onClick.AddListener(TestEndGame);

		base.Awake(); //always call base.event() at the end
		OnAwaken();
	}

	private void OnAwaken()
	{
		mainCamera.enabled = false;
		//todo: deactivate players
	}

	public new void Activate()
	{
		Debug.Log("Game Activate");
		base.Activate();
		mainCamera.enabled = true;
	}

	public void TestEndGame()
	{
		brainiacs.SetGameResultInfo(PlayerManager.Players);
		brainiacs.Scenes.LoadScene(EScene.Results);
	}
}
