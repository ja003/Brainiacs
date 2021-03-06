﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerInfoController : GameController
{
	[SerializeField] private UIPlayerInfoElement prefab = null;

	protected override void OnMainControllerAwaken()
	{
		//client has to init after all players are added
		game.PlayerManager.OnAllPlayersAdded.AddAction(InitPlayersInfo);
	}

	protected override void OnMainControllerActivated()
	{
	}

	/// <summary>
	/// Called after all players are added
	/// </summary>
	private void InitPlayersInfo()
	{
		foreach(var player in game.PlayerManager.Players)
		{
			AddPlayerInfo(player);
		}
		prefab.gameObject.SetActive(false);
	}

	private void AddPlayerInfo(Player pPlayer)
	{
		UIPlayerInfoElement instance = Instantiate(prefab, transform);
		instance.Init(pPlayer);
	}
	
}
