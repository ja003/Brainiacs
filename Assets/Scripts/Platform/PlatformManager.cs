using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlatformManager
{
	static bool debugMobile = false;
	
	public static EPlatform GetPlatform()
	{		
		if(Application.isMobilePlatform || debugMobile)
		{
			return EPlatform.Mobile;
		}

		return EPlatform.PC;
	}

	public static bool IsMobile()
	{
		return GetPlatform() == EPlatform.Mobile;
	}
}

public enum EPlatform
{
	None, Mobile, PC
}