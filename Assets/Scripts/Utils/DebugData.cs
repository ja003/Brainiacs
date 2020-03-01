using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class DebugData
{
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

	public static PlayerInitInfo GetPlayerInitInfo(int pIndex)
	{
		PlayerInitInfo player = null;
		switch(pIndex)
		{
			case 1:
				player = new PlayerInitInfo(
			EHero.Nobel, GetPlayerName(pIndex), EPlayerColor.Green);
				player.PlayerKeys = new PlayerKeys(
					KeyCode.UpArrow, KeyCode.RightArrow,
					KeyCode.DownArrow, KeyCode.LeftArrow,
					KeyCode.RightControl, KeyCode.RightShift);
				break;
			case 2:
				player = new PlayerInitInfo(
			EHero.Einstein, GetPlayerName(pIndex), EPlayerColor.Pink);
				player.PlayerKeys = new PlayerKeys(
					KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A,
					KeyCode.LeftControl, KeyCode.LeftShift);
				break;
			case 3:
				player = new PlayerInitInfo(
			EHero.Currie, GetPlayerName(pIndex), EPlayerColor.Yellow);

				player.PlayerKeys = new PlayerKeys(
					KeyCode.Alpha8, KeyCode.Alpha6, KeyCode.Alpha5, KeyCode.Alpha4,
					KeyCode.Alpha1, KeyCode.Alpha3);
				break;
		}

		return player;


	}
}

