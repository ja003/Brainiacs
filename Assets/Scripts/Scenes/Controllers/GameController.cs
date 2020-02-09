using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController : GameBehaviour
{
	protected override void Awake()
	{
		//Debug.Log($"{gameObject.name} Awake");
		base.Awake();
		if(game != this)
		{
			game.SetOnAwaken(GameAwaken);
			game.SetOnActivated(GameActivated);
		}
	}

	bool onGameAwakenCalled = false;
	private void GameAwaken()
	{
		if(onGameAwakenCalled)
		{
			Debug.LogError($"{gameObject.name} GameAwaken already called");
			return;
		}
		OnGameAwaken();
		onGameAwakenCalled = true;
	}

	protected abstract void OnGameAwaken();

	bool onGameActivatedCalled = false;
	private void GameActivated()
	{
		//Debug.Log($"{gameObject.name} GameActivated");

		if(onGameActivatedCalled)
		{
			Debug.LogError($"{gameObject.name} GameActivated already called");
			return;
		}
		OnGameActivated();
		onGameActivatedCalled = true;
	}
	protected abstract void OnGameActivated();

	public virtual Transform GetHolder()
	{
		return transform;
	}

	//public Action OnActivated;

}
