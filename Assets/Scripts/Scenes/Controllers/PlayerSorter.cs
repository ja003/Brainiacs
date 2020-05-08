using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSorter : BrainiacsBehaviour
{
	private List<Player> players = new List<Player>();

	public void SetPlayers(List<Player> pPlayers)
	{
		players = pPlayers;
	}

	private void Update()
	{
		//sort players based on their Y position
		//lower Y => higher layer order
		players.Sort((b, a) => a.transform.position.y.CompareTo(b.transform.position.y));

		for(int i = 0; i < players.Count; i++)
		{
			players[i].Visual.UpdateSortOrder(i);
		}
	}
}
