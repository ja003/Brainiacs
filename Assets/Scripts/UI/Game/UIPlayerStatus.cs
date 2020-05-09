using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatus : UiBehaviour
{
	[SerializeField] private Image icon = null;
	[SerializeField] private Text text = null;

	public void SpawnAt(Vector3 pWorldPosition, Sprite pSprite, string pText, Color? pTextColor = null)
	{
		gameObject.SetActive(true);

		Vector2 screenPosition = GetScreenPosition(pWorldPosition);

		transform.localScale = Vector3.one; //scale bug (sometimes it is changed?)
		rectTransform.anchoredPosition = screenPosition;

		//Debug.Log($"Screen {Screen.width} x {Screen.height}");
		//Debug.Log($"SpawnAt {viewportPos} => {screenPosition.x} ; {screenPosition.y}");

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

	internal void debug_SpawnAt(Vector3 pPos)
	{
		Debug.Log("debug_SpawnAt " + pPos);
		gameObject.SetActive(true);
		Vector2 screenPosition = GetScreenPosition(pPos);

		icon.color = Color.red;
		rectTransform.anchoredPosition = screenPosition;
	}

	private Vector2 GetScreenPosition(Vector3 pWorldPos)
	{
		Utils.DebugDrawCross(pWorldPos, Color.red);

		Vector3 canvasScale = game.UiCanvas.transform.localScale;
		Vector2 viewportPos = Camera.main.WorldToViewportPoint(pWorldPos);

		float x = viewportPos.x * Screen.width - Screen.width / 2;
		float y = viewportPos.y * Screen.height - Screen.height / 2;
		x /= canvasScale.x;
		y /= canvasScale.y;

		Vector2 screenPosition = new Vector2(x, y);
		return screenPosition;
	}

	public void anim_OnAnimFinished()
	{
		//Debug.Log("anim_OnAnimFinished");
		Deactivate();
	}

	private void Deactivate()
	{
		InstanceFactory.Destroy(gameObject, false);
		//gameObject.SetActive(false);
	}
}
