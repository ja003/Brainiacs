using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tutorial : BrainiacsBehaviour
{
	//usually tehre is only 1 object focused, so this is special field for it.
	//special objects arte defined in additionalFocused and all operations are
	//done on that list
	[SerializeField] GameObject focused;
	[SerializeField] List<GameObject> additionalFocused;

	[SerializeField] List<Button> targetBtns;
	[SerializeField] protected Tutorial next;

	//part completed by this tutorial. OnCompleted gets called
	[SerializeField] protected ETutorial tutorialPart;

	[SerializeField] bool skipAddingCanvas;

	[SerializeField] EPlatform targetPlatform;


	protected override void Awake()
	{
		if(focused != null)
			additionalFocused.Add(focused);

		foreach(Button btn in targetBtns)
		{
			btn.onClick.AddListener(OnTargetBtnClick);
		}
		base.Awake();
	}

	public new void Activate()
	{
		//if target platform is specified and does not match => skip to next
		if(targetPlatform != EPlatform.None && targetPlatform != PlatformManager.GetPlatform())
		{
			next?.Activate();
			return;
		}

		//Debug.Log($"Activate {gameObject.name}");
		gameObject.SetActive(true);
		SetBgEnabled(true);
		BringFocusedToFront();
		OnActivated();
	}

	protected abstract void OnActivated();

	protected abstract void SetBgEnabled(bool pValue);

	private void BringFocusedToFront()
	{
		foreach(GameObject foc in additionalFocused)
		{
			var canvas = skipAddingCanvas ? foc.GetComponent<Canvas>() : foc.AddComponent<Canvas>();
			foc.AddComponent<GraphicRaycaster>();
			canvas.overrideSorting = true;
			canvas.sortingOrder = 10;
			canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
		}
	}

	private void RevertFocused()
	{
		foreach(GameObject foc in additionalFocused)
		{
			Destroy(foc.GetComponent(typeof(GraphicRaycaster)));
			if(!skipAddingCanvas)
				Destroy(foc.GetComponent(typeof(Canvas)));
		}
	}

	public void Deactivate()
	{
		if(!gameObject.activeSelf)
		{
			//todo: after click on heroSelect - next, OnTargetBtnClick
			//is called twice
			Debug.LogError($"Already deactivated {gameObject.name}");
			return;
		}

		//Debug.Log($"Deactivate {gameObject.name}");

		RevertFocused();
		SetBgEnabled(false);
		gameObject.SetActive(false);
		next?.Activate();

		OnCompleted();
	}

	protected abstract void OnCompleted();

	private void OnTargetBtnClick()
	{
		foreach(Button btn in targetBtns)
		{
			btn.onClick.RemoveListener(OnTargetBtnClick);
		}

		Deactivate();
	}

	internal void SetFocusedObject(GameObject pFocused)
	{
		additionalFocused.Add(pFocused);
	}

	internal void SetTargetBtn(Button pTargetBtn)
	{
		//Debug.Log($"SetTargetBtn {pTargetBtn.gameObject.name}");
		targetBtns.Add(pTargetBtn);
		pTargetBtn.onClick.AddListener(OnTargetBtnClick);
	}
}
