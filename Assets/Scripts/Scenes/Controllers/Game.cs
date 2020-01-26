using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : CSingleton<Game>
{
	[SerializeField]
	Camera mainCamera;

	[SerializeField]
	MapController map;

	private List<Player> players;


	protected override void Awake()
	{
		base.Awake();
		OnLoaded();
		if(Brainiacs.SelfInitGame)
		{
			Activate();
		}
	}

	public void OnLoaded()
	{
		mainCamera.enabled = false;
		map.SetMap(brainiacs.GameInitInfo.Map);

		players = new List<Player>();
		foreach(PlayerInitInfo playerInfo in brainiacs.GameInitInfo.players)
		{
			players.Add(new Player(playerInfo.Lives, playerInfo.Hero, playerInfo.Name));
		}

		map.SetActive(false);
		//todo: deactivate players
	}

	public void Activate()
	{
		mainCamera.enabled = true;
		map.SetActive(true);
		//todo: activate players
	}

	public void TestEndGame()
	{
		brainiacs.SetGameResultInfo(players);
		brainiacs.Scenes.LoadScene(EScene.Results);
	}
}
