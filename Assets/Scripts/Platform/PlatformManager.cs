using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlatformManager
{
	static bool debugMobile = true;
	
	public static EPlatform GetPlatform()
	{
		RuntimePlatform platform = Application.platform;
		if(platform == RuntimePlatform.Android ||
			platform == RuntimePlatform.IPhonePlayer ||
			debugMobile)
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