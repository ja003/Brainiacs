using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExtendedButton : Button
{
	public Action OnPointerDownAction;
	public Action OnPointerUpAction;
	public Action OnPressedAction;

	public bool IsPressed { get; private set; }

	private void FixedUpdate()
	{
		if(IsPressed())
		{
			OnPressedAction?.Invoke();
		}
	}


	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		IsPressed = true;
		OnPointerDownAction?.Invoke();
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		IsPressed = false;
		OnPointerUpAction?.Invoke();
	}

}


