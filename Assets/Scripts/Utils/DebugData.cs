﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class DebugData
{
	public static bool TestRemote = false;

	public static bool TestMP = false;

	public static void OnBrainiacsAwake()
	{
		if(TestRemote)
		{
			Debug.LogError("Testing remote");
		}
		if(TestMP)
		{
			Debug.LogError("DebugMP");
		}
	}

	public static string GetPlayerName(int pIndex)
	{
		switch(pIndex)
		{
			case 1:
				return "Adam";
			case 2:
				return "Téra";
			case 3:
				return "Johanka";
		}
		return "DEBUG_NAME";
	}

	public static PlayerInitInfo GetPlayerInitInfo(int pPlayerNumber)
	{
		PlayerInitInfo player = null;
		switch(pPlayerNumber)
		{
			case 1:
				player = new PlayerInitInfo(pPlayerNumber,
			EHero.Nobel, GetPlayerName(pPlayerNumber),
			EPlayerColor.Green, EPlayerType.LocalPlayer);
				//todo: implement debug data for PC and Unity platform
			//DebugData.TestMP ? EPlayerType.LocalPlayer : EPlayerType.);
				//player.PlayerKeys = new PlayerKeys(
				//	KeyCode.UpArrow, KeyCode.RightArrow,
				//	KeyCode.DownArrow, KeyCode.LeftArrow,
				//	KeyCode.RightControl, KeyCode.RightShift);
				break;
			case 2:
				player = new PlayerInitInfo(pPlayerNumber,
			EHero.Einstein, GetPlayerName(pPlayerNumber),
			EPlayerColor.Pink, EPlayerType.LocalPlayer);
				//player.PlayerKeys = new PlayerKeys(
				//	KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A,
				//	KeyCode.LeftControl, KeyCode.LeftShift);
				break;
			case 3:
				player = new PlayerInitInfo(pPlayerNumber,
			EHero.Currie, GetPlayerName(pPlayerNumber),
			EPlayerColor.Yellow, EPlayerType.LocalPlayer);

				//player.PlayerKeys = new PlayerKeys(
				//	KeyCode.Alpha8, KeyCode.Alpha6, KeyCode.Alpha5, KeyCode.Alpha4,
				//	KeyCode.Alpha1, KeyCode.Alpha3);
				break;
		}

		return player;


	}
}

