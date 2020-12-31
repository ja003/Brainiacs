using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideForPlatform : BrainiacsBehaviour
{
	[SerializeField] EPlatform platform;

	[SerializeField] CanvasGroup canvasGroup;

	void Start()
	{
		bool enable = true;
		switch(platform)
		{
			case EPlatform.Mobile:
				enable = !PlatformManager.IsMobile();
				break;
			case EPlatform.PC:
				enable = PlatformManager.IsMobile();
				break;
		}

		canvasGroup.alpha = enable ? 1 : 0;
		canvasGroup.blocksRaycasts = enable;
		//gameObject.SetActive(enable);
	}
}
