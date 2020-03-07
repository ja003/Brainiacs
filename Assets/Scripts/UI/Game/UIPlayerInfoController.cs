using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerInfoController : GameController
{
	[SerializeField] private UIPlayerInfoElement prefab;

	protected override void OnMainControllerAwaken()
	{
		game.playersController.SetOnActivated(InitPlayersInfo);
	}

	protected override void OnMainControllerActivated()
	{
	}

	private void InitPlayersInfo()
	{
		foreach(var player in game.playersController.Players)
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
