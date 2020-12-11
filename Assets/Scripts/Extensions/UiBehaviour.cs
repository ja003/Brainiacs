using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiBehaviour : BrainiacsBehaviour
{
	protected Game game => Game.Instance;

	protected Vector2 GetScreenPosition(Vector2 pWorldPos)
	{
		//Utils.DebugDrawCross(pWorldPos, Color.red);

		Vector2 canvasScale = game.UiCanvas.transform.localScale;
		Vector2 viewportPos = Camera.main.WorldToViewportPoint(pWorldPos);

		float x = viewportPos.x * Screen.width - Screen.width / 2;
		float y = viewportPos.y * Screen.height - Screen.height / 2;
		x /= canvasScale.x;
		y /= canvasScale.y;

		Vector2 screenPosition = new Vector2(x, y);
		return screenPosition;
	}

	public void SetPosition(Vector2 pWorldPosition)
	{
		Vector2 screenPosition = GetScreenPosition(pWorldPosition);

		transform.localScale = Vector2.one; //scale bug (sometimes it is changed?)
		rectTransform.anchoredPosition = screenPosition;
	}

	/// <summary>
	/// Instantly sets alpha to image color property
	/// </summary>
	protected void SetAlpha(float pAlpha)
	{
		image.enabled = true;
		image.color = new Color(image.color.r, image.color.g, image.color.b, pAlpha);
	}

	/// <summary>
	/// Animates alpha value of image over given duration and calls action on complete
	/// </summary>
	protected void SetAlpha(float pValue, float pDuration, Action pOnComplete)
	{
		LeanTween.alpha(image.rectTransform, pValue, pDuration)
					.setIgnoreTimeScale(true)
					.setOnComplete(pOnComplete);
	}

	private RectTransform _rectTransform;
	protected RectTransform rectTransform
	{
		get
		{
			if(_rectTransform == null)
				_rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}
	}

	private Image _image;
	protected Image image
	{
		get
		{
			if(_image == null)
				_image = GetComponent<Image>();
			return _image;
		}
	}

	private Button _button;
	protected Button button
	{
		get
		{
			if(_button == null)
				_button = GetComponent<Button>();
			return _button;
		}
	}

}
