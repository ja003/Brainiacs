using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://docs.google.com/spreadsheets/d/1hI2HANvhAhnk5RRNRqE4HF329iWowX1vS71R1bBqEtY/edit#gid=0
/// </summary>
public static class SortLayerManager
{
	//const int player_sort_index_start = 10;

	/// <summary>
	/// Standart order:
	/// - Player, Weapon, Hands
	/// Direction up: (handled separately in other methods)
	/// - Weapon, Hands, Player
	/// 
	/// Players are sorted based on their Y position.
	/// Each player has reserved layer indexes based on their order.
	/// </summary>
	public static int GetPlayerSortIndexStart(PlayerVisual.ESortLayer pLayer, int pCurrentSortOrder)
	{
		int index = GetSortIndex(ESortObject.Player) + pCurrentSortOrder * GetSortIndex(ESortObject.Player);
		switch(pLayer)
		{
			case PlayerVisual.ESortLayer.Player:
				break;
			case PlayerVisual.ESortLayer.Weapon:
				index += 1;
				break;
			case PlayerVisual.ESortLayer.Hands:
				index += 2;
				break;
		}
		//Debug.Log(pLayer + " order = " + index);

		return index;
	}

	public static int GetSortIndex(ESortObject pSortObject)
	{
		switch(pSortObject)
		{
			case ESortObject.Background:
				return -10;
			case ESortObject.MapObject:
				return -5;
			case ESortObject.Foreground:
				return -1;
			case ESortObject.Player:
				return 10;
			case ESortObject.Flamethrower:
				return 100;
		}
		Debug.LogError("Unknown sort object " + pSortObject);
		return 0;
	}
}


public enum ESortObject
{
	None,
	Background,
	MapObject,
	Foreground,
	Player,
	Flamethrower
}