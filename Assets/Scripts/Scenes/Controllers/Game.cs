using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : CSingleton<Game>
{
	[SerializeField]
	Camera mainCamera;

	[SerializeField]
	GameObject map;



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
		brainiacs.Scenes.LoadScene(EScene.Results);
	}
}
