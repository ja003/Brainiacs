using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreboardController : GameController
{
	[SerializeField] private UIScoreboardElement scorePlayerPrefab = null;
	[SerializeField] Image background;

	protected override void Awake()
	{
		base.Awake();
		game.Lighting.RegisterForLighting(background);
	}

	protected override void OnMainControllerAwaken()
	{
		game.PlayerManager.OnAllPlayersAdded.AddAction(InitPlayersScore);
	}

	protected override void OnMainControllerActivated()
	{
	}

	/// <summary>
	/// Called after all players are added
	/// </summary>
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
