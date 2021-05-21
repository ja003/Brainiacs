﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonObject : UiBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public Action<float> OnPointerUpAction; //arg = hold duration
	public Action<float> OnPointerHoldUpdate; //arg = hold duration

	protected override void Awake()
	{
		button.onClick.AddListener(OnClick);
		base.Awake();
	}

	private void Update()
	{
		if(pointerDownTime < 0)
			return;

		OnPointerHoldUpdate?.Invoke(Time.time - pointerDownTime);
	}

	public bool IsClicked()
	{
		return pointerDownTime > 0;
	}

	private void OnClick()
	{
		brainiacs.AudioManager.PlaySound(ESound.Ui_Button_Click, null);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		brainiacs.AudioManager.PlaySound(ESound.Ui_Button_Hover, null);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		brainiacs.AudioManager.PlaySound(ESound.Ui_Button_Hover, null);
	}

	float pointerDownTime = -1;
	public void OnPointerDown(PointerEventData eventData)
	{
		pointerDownTime = Time.time;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		OnPointerUpAction?.Invoke(Time.time - pointerDownTime);
		pointerDownTime = -1;
	}
}
