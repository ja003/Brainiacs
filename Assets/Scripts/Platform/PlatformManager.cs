using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlatformManager
{
	public static EPlatform GetPlatform()
	{		
		if(IsMobile())
			return EPlatform.Mobile;

		return EPlatform.PC;
	}

	public static bool IsMobile()
	{
		return Application.isMobilePlatform || CDebug.Instance.PlatformMobile;
	}
}

public enum EPlatform
{
	None, Mobile, PC
}