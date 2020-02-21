using System;
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

	protected override void OnGameAwaken()
	{
		game.playersController.SetOnActivated(InitPlayersScore);
	}

	protected override void OnGameActivated()
	{
	}

	private void InitPlayersScore()
	{
		foreach(var player in game.playersController.Players)
		{
			AddScorePlayer(player.Stats);
		}
		scorePlayerPrefab.gameObject.SetActive(false);
	}


	private void AddScorePlayer(PlayerStats pPlayerStats)
	{
		UIScoreboardElement instance = Instantiate(scorePlayerPrefab, transform);
		instance.Init(pPlayerStats);
	}
}
