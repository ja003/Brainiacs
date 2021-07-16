using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Tutorial : BrainiacsBehaviour
{
	//usually there is only 1 object focused, so this is special field for it.
	//special objects are defined in additionalFocused and all operations are
	//done on that list
	[SerializeField] GameObject focused;
	[SerializeField] List<GameObject> additionalFocused;

	//buttons we want user to click
	[SerializeField] List<Button> targetBtns;
	[SerializeField] protected Tutorial next;

	//part completed by this tutorial. OnCompleted gets called
	[SerializeField] protected ETutorial tutorialPart;

	[FormerlySerializedAs("skipAddingCanvas")]
	//skip adding (and then removing) canvas and graphic raycaster
	//focused element should be already in good state for tutorial
	[SerializeField] bool skipAddingFocusComponent;

	[SerializeField] EPlatform targetPlatform;

	UiCopyPosition uiCopyPosition;

	protected override void Awake()
	{
		uiCopyPosition = GetComponent<UiCopyPosition>();

		if(focused != null)
			additionalFocused.Add(focused);

		//in case SetTargetBtn was called before Awake => remove
		//all listeners and add them again (easier than keeping that extra info)
		foreach(Button btn in targetBtns)
			btn.onClick.RemoveListener(OnTargetBtnClick);

		foreach(Button btn in targetBtns)
			btn.onClick.AddListener(OnTargetBtnClick);

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

		//update the position (if it has the script) or there is a 1 frame lag
		uiCopyPosition?.Update();

		//Debug.Log($"Activate {gameObject.name}");
		gameObject.SetActive(true);
		SetBgEnabled(true);
		//BringFocusedToFront();
		//show with small delay
		//looks better plus there is a problem when removing and adding GraphicRaycaster
		//from the same object in 2 following Tutorials
		DoInTime(() => BringFocusedToFront(), 0.2f, true);

		OnActivated();
	}

	protected abstract void OnActivated();

	protected abstract void SetBgEnabled(bool pValue);

	private void BringFocusedToFront()
	{
		if(skipAddingFocusComponent)
			return;

		foreach(GameObject foc in additionalFocused)
		{
			var canvas = foc.GetComponent<Canvas>() != null ?
				foc.GetComponent<Canvas>() : foc.AddComponent<Canvas>();
			foc.AddComponent<GraphicRaycaster>();
			canvas.overrideSorting = true;
			canvas.sortingOrder = 10;
			canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
		}
	}

	private void RevertFocused()
	{
		if(skipAddingFocusComponent)
			return;

		foreach(GameObject foc in additionalFocused)
		{
			Destroy(foc.GetComponent(typeof(GraphicRaycaster)));
			if(!skipAddingFocusComponent)
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

		//DoInTime(() => next?.Activate(), 0.2f);
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
