using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeysManager : MonoBehaviour
{
	List<PlayerKeys> keys;

	private void Awake()
	{
		//TODO: load from DB
		keys = new List<PlayerKeys>();
		//1
		keys.Add(new PlayerKeys(
			KeyCode.W, KeyCode.D, 
			KeyCode.S, KeyCode.A,
			KeyCode.LeftControl, KeyCode.LeftShift));
		//2
		keys.Add(new PlayerKeys(
			KeyCode.UpArrow, KeyCode.RightArrow, 
			KeyCode.DownArrow, KeyCode.LeftArrow,
			KeyCode.RightControl, KeyCode.RightShift));
		//3
		keys.Add(new PlayerKeys(
			KeyCode.I, KeyCode.L,
			KeyCode.K, KeyCode.J,
			KeyCode.O, KeyCode.P));
	}

	public PlayerKeys GetPlayerKeys(int pPlayerNumber)
	{
		if(pPlayerNumber > keys.Count)
		{
			Debug.LogError($"Keys for player {pPlayerNumber} not configured");
			return GetPlayerKeys(pPlayerNumber % keys.Count);
		}

		return keys[pPlayerNumber - 1];
	}

}
