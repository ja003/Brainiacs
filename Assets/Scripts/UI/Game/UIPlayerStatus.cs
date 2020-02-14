using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatus : UiBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private Text text;

	public void SpawnAt(Vector3 pWorldPosition, Sprite pSprite, string pText)
	{
		Vector2 viewportPos = Camera.main.WorldToViewportPoint(pWorldPosition);
		float x = viewportPos.x * Screen.width - Screen.width / 2;
		float y = viewportPos.y * Screen.height - Screen.height / 2;
		Vector2 screenPosition = new Vector2(x, y);

		rectTransform.anchoredPosition = screenPosition;

		//Debug.Log($"Screen {Screen.width} x {Screen.height}");
		//Debug.Log($"SpawnAt {viewportPos} => {screenPosition.x} ; {screenPosition.y}");

		if(pSprite != null)
			icon.sprite = pSprite;
		text.text = pText;

		//TODO: make general for UI behaviour?
		animator.SetTrigger("play");

		//DOES NOT WORK
		//- moves in other direction (no matter the value)
		//- doesnt work correctly with non-ONE canvas scale
		//LeanTween.moveY(gameObject, -1, 1).setOnComplete(Deactivate);
	}

	public void anim_OnAnimFinished()
	{
		//Debug.Log("anim_OnAnimFinished");
		Deactivate();
	}

	private void Deactivate()
	{
		gameObject.SetActive(false);
	}
}
