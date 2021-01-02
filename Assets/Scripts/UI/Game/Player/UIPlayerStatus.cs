using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatus : UiBehaviour
{
	[SerializeField] private Image icon = null;
	[SerializeField] private Text text = null;

	public void SpawnAt(Vector2 pWorldPosition, Sprite pSprite, string pText, Color? pTextColor = null)
	{
		gameObject.SetActive(true);

		SetPosition(pWorldPosition);

		//Vector2 screenPosition = GetScreenPosition(pWorldPosition);
		//transform.localScale = Vector2.one; //scale bug (sometimes it is changed?)
		//rectTransform.anchoredPosition = screenPosition;

		//Debug.Log($"Screen {Screen.width} x {Screen.height}");
		//Debug.Log($"SpawnAt {pWorldPosition} => {pText}");

		//if(pSprite != null)
		icon.sprite = pSprite;
		icon.enabled = pSprite != null;

		text.text = pText;
		text.color = pTextColor == null ? Color.red : (Color)pTextColor;

		//TODO: make general for UI behaviour?
		animator.SetTrigger("play");

		//DOES NOT WORK
		//- moves in other direction (no matter the value)
		//- doesnt work correctly with non-ONE canvas scale
		//LeanTween.moveY(gameObject, -1, 1).setOnComplete(Deactivate);
	}

	internal void debug_SpawnAt(Vector2 pPos)
	{
		Debug.Log("debug_SpawnAt " + pPos);
		gameObject.SetActive(true);
		Vector2 screenPosition = GetScreenPosition(pPos);

		icon.color = Color.red;
		rectTransform.anchoredPosition = screenPosition;
	}

	

	public void anim_OnAnimFinished()
	{
		//Debug.Log("anim_OnAnimFinished");
		Deactivate();
	}

	private void Deactivate()
	{
		InstanceFactory.Destroy(gameObject);//, false);
		//gameObject.SetActive(false);
	}
}
