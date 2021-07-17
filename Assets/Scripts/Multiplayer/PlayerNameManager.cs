using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerNameManager
{
	public static string GetPlayerName(int pNumber)
	{
		string player_name_key = GetPlayerNameKey(pNumber);
		if(PlayerPrefs.HasKey(player_name_key))
		{
			return PlayerPrefs.GetString(player_name_key);
		}
		else
		{
			string generatedName = GenerateName(pNumber);
			SaveName(pNumber, generatedName);
			return generatedName;
		}
	}

	private static string GetPlayerNameKey(int pNumber)
	{
		return "playerName_" + pNumber;
	}

	public static void SaveName(int pNumber, string pName)
	{
		PlayerPrefs.SetString(GetPlayerNameKey(pNumber), pName);
	}

	private static string GenerateName(int pNumber)
	{
		return "player_" + pNumber;
	}
}
