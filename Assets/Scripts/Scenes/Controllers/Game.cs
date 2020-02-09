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
	public PlayersController playersController;

	[SerializeField]
	public ProjectileManager ProjectileManager;

	protected override void Awake()
	{
		base.Awake();
		OnAwaken();
		if(Brainiacs.SelfInitGame)
		{
			DoInTime(Activate, 0.5f);
		}
	}

	public void OnAwaken()
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
		brainiacs.SetGameResultInfo(playersController.Players);
		brainiacs.Scenes.LoadScene(EScene.Results);
	}
}
