using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : CSingleton<Game>
{
	[SerializeField]
	Camera mainCamera;

	[SerializeField]
	public MapController MapController;

	[SerializeField]
	private PlayersController playersController;



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
		MapController.SetMap(brainiacs.GameInitInfo.Map);

		MapController.SetActive(false);

		playersController.SpawnPlayers(brainiacs.GameInitInfo.players);
		//todo: deactivate players


	}

	public void Activate()
	{
		mainCamera.enabled = true;
		MapController.SetActive(true);
		//todo: activate players
	}

	public void TestEndGame()
	{
		brainiacs.SetGameResultInfo(playersController.Players);
		brainiacs.Scenes.LoadScene(EScene.Results);
	}
}
