using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Provides action calls of main scene controller life-cycle events
/// </summary>
public abstract class ControllerBehaviour : BrainiacsBehaviour
{
	/// <summary>
	/// Scenes:
	/// - MainMenu
	/// - Game
	/// </summary>
	protected abstract BrainiacsBehaviour GetMainController();

	protected override void Awake()
	{
		//Debug.Log($"{gameObject.name} Awake");
		base.Awake();
		if(GetMainController() != this)
		{
			GetMainController().SetOnAwaken(MainControllerAwaken);
			GetMainController().SetOnActivated(MainControllerActivated);
		}
	}

	bool onMainControllerAwakenCalled = false;
	private void MainControllerAwaken()
	{
		if(onMainControllerAwakenCalled)
		{
			Debug.LogError($"{gameObject.name} MainControllerAwaken already called");
			return;
		}
		OnMainControllerAwaken();
		onMainControllerAwakenCalled = true;
	}

	protected abstract void OnMainControllerAwaken();

	bool onMainControllerActivatedCalled = false;
	private void MainControllerActivated()
	{
		//Debug.Log($"{gameObject.name} GameActivated");

		if(onMainControllerActivatedCalled)
		{
			Debug.LogError($"{gameObject.name} MainControllerActivated already called");
			return;
		}
		OnMainControllerActivated();
		onMainControllerActivatedCalled = true;
	}
	protected virtual void OnMainControllerActivated() { }
}