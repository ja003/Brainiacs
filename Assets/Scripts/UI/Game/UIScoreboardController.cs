﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScoreboardController : GameController
{
	[SerializeField]
	private UIScoreboardElement scorePlayerPrefab;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnMainControllerAwaken()
	{
		game.PlayerManager.OnAllPlayersAdded.AddAction(InitPlayersScore);
	}

	protected override void OnMainControllerActivated()
	{
	}

	private void InitPlayersScore()
	{
		foreach(var player in game.PlayerManager.Players)
		{
			AddScorePlayer(player);
		}
		scorePlayerPrefab.gameObject.SetActive(false);
	}


	private void AddScorePlayer(Player pPlayer)
	{
		UIScoreboardElement instance = Instantiate(scorePlayerPrefab, transform);
		instance.Init(pPlayer);
	}
}
