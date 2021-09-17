﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows health of an object in game.
/// Automatically updates its position to fit its owner.
/// </summary>
public class UIHealthbar : UiBehaviour
{
	[SerializeField] Slider slider;
	[SerializeField] Image background;
	[SerializeField] Image fill;

	private GameBehaviour owner;
	[SerializeField] private Vector2 offset;

	public bool IsVisible { get; private set; }

	bool isInited;

	public void Init(GameBehaviour pOwner, Vector2 pOffset, bool pInitVisible, Color pColor)
	{
		isInited = true;
		owner = pOwner;
		offset = pOffset;
		gameObject.SetActive(true);
		IsVisible = pInitVisible;
		SetColor(pColor);
	}

	private void Update()
	{
		if(!isInited || Camera.main == null)
			return;

		if(!brainiacs.PlayerPrefs.ShowHealthbars || !IsVisible)
		{
			SetVisible(false);
			return;
		}

		SetVisible(true);
		SetPosition(owner.Position2D + offset);
	}

	public void SetHealth(int pHealth, int pMaxHealth)
	{
		slider.maxValue = pMaxHealth;
		slider.value = pHealth;
	}

	internal void SetVisibility(bool pValue)
	{
		IsVisible = pValue;
		//force position update, or there is 1 frame delay
		Update();
	}

	internal void SetColor(Color pColor)
	{
		fill.color = pColor;
		background.color = pColor;
	}

	void SetVisible(bool pValue)
	{
		background.enabled = pValue;
		fill.enabled = pValue;
	}
}