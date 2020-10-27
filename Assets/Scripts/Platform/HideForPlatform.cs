using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideForPlatform : MonoBehaviour
{
	[SerializeField] EPlatform platform;

	[SerializeField] CanvasGroup canvasGroup;

	void Start()
	{
		bool enable = true;
		switch(platform)
		{
			case EPlatform.Mobile:
				enable = !Application.isMobilePlatform;
				break;
			case EPlatform.PC:
				enable = Application.isMobilePlatform;
				break;
		}

		canvasGroup.alpha = enable ? 1 : 0;
		canvasGroup.blocksRaycasts = enable;
		//gameObject.SetActive(enable);
	}
}
