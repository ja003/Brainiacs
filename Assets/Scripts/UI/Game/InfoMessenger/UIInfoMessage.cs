using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Info message in game. Handled by UIInfoMessenger
/// </summary>
public class UIInfoMessage : UiBehaviour
{
	private const float VISIBLE_ALPHA = 0.8f;
	[SerializeField] Text msgText;

	private new void Awake()
	{
		SetAlpha(0);
		//bug: when 3rd msg is instantiated it has larger scale => set manually
		rectTransform.localScale = Vector3.one;
	}

	public void Show(string pText, float pDuration = 3)
	{
		gameObject.SetActive(true);
        transform.SetAsFirstSibling(); //new msgs will be on top
		msgText.text = pText;
		SetAlpha(0);
		SetAlpha(VISIBLE_ALPHA, 0.3f, () => DoInTime(Hide, pDuration));
	}

	private void Hide()
	{
		SetAlpha(0, 0.1f, () => GetComponent<PoolObject>().ReturnToPool());
	}

	/// <summary>
	/// There is artefact if text alpha is not set as well
	/// </summary>
	private new void SetAlpha(float pAlpha)
	{
		base.SetAlpha(pAlpha);
		msgText.color = new Color(msgText.color.r, msgText.color.g, msgText.color.b, pAlpha);
	}
}
