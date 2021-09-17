using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Any UI menu (in game or mainMenu)
/// </summary>
public abstract class MenuController : ControllerBehaviour
{
	[SerializeField] CanvasGroup canvasGroup;

	protected override void Awake()
	{
		base.Awake();

		canvasGroup = Holder.GetComponent<CanvasGroup>();
		if(canvasGroup == null)
		{
			canvasGroup = Holder.AddComponent<CanvasGroup>();
			Debug.LogWarning($"Add CanvasGroup to {gameObject.name}");
		}

		//refresh active state (especially blocksRaycasts)
		SetActive(canvasGroup.alpha > 0);
	}

	public override void SetActive(bool pValue)
	{
		canvasGroup.blocksRaycasts = pValue;
		canvasGroup.interactable = pValue;
		
		if(animator)
			animator.SetBool("isActive", pValue);
		else
			canvasGroup.alpha = pValue ? 1 : 0;

		if(!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
			Debug.LogError($"Menu {gameObject.name} was not active");
		}
	}

	public override bool IsActive()
	{
		return base.IsActive() && canvasGroup.alpha > 0.1f;
	}
}