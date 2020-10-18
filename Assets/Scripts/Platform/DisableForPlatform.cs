using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableForPlatform : MonoBehaviour
{
	[SerializeField]
	EPlatform platform;

	// Start is called before the first frame update
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

		gameObject.SetActive(enable);
	}
}
