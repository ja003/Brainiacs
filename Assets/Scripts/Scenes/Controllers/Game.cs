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
	public PlayerManager playersController;

	[SerializeField]
	public ProjectileManager ProjectileManager;

	[SerializeField]
	public UIPlayerStatusManager PlayerStatusManager;

	protected override void Awake()
	{		
		if(Brainiacs.SelfInitGame)
		{
			DoInTime(Activate, 0.5f);
		}

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
		brainiacs.SetGameResultInfo(playersController.Players);
		brainiacs.Scenes.LoadScene(EScene.Results);
	}
}
