using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
	public static class MyExtensions
	{
		public static int WordCount(this String str)
		{
			return str.Split(new char[] { ' ', '.', '?' },
							 StringSplitOptions.RemoveEmptyEntries).Length;
		}

		public static string Colorify(this String pText, Color pColor)
		{
			return $"<color=#{ColorUtility.ToHtmlStringRGB(pColor)}>{pText}</color>";
		}
	}
}
