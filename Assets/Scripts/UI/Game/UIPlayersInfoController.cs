using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayersInfoController : GameController
{
	[SerializeField] private UIPlayerInfo prefab;

	protected override void OnGameAwaken()
	{
		game.playersController.SetOnActivated(InitPlayersInfo);
	}

	protected override void OnGameActivated()
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
		UIPlayerInfo instance = Instantiate(prefab, transform);
		instance.Init(pPlayer);
	}
	
}
