﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Provides action calls of main scene controller life-cycle events
/// </summary>
public abstract class ControllerBehaviour : BrainiacsBehaviour
{
	[Header("Controller")]
	[SerializeField] private GameObject holder = null;
	protected GameObject Holder
	{
		get { return holder == null ? gameObject : holder; }
	}

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

		BrainiacsBehaviour mainController = GetMainController();
		if(mainController != null && mainController != this)
		{
			mainController.SetOnAwaken(MainControllerAwaken);
			mainController.SetOnActivated(MainControllerActivated);
		}
	}

	public void SetActive(bool pValue)
	{
		Holder.SetActive(pValue);
		OnSetActive(pValue);
	}

	protected virtual void OnSetActive(bool pValue) { }

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