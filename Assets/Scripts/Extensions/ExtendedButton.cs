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
		
		OnPointerDownAction?.Invoke();
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);

		OnPointerUpAction?.Invoke();
	}
}


