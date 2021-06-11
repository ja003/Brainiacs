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
	}

	public override void SetActive(bool pValue)
	{
		if(animator)
		{
			canvasGroup.interactable = pValue;
			canvasGroup.blocksRaycasts = pValue;
			animator.SetBool("isActive", pValue);
		}
		else
		{
			canvasGroup.alpha = pValue ? 1 : 0;
		}

	}

	public override bool IsActive()
	{
		return base.IsActive() && canvasGroup.alpha > 0.1f;
	}
}