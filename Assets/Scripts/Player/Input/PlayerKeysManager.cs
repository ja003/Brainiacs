using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeysManager : MonoBehaviour
{
	List<PlayerKeys> keys;

	Dictionary<int, PlayerKeys> assignedKeys;

	private void Awake()
	{
		assignedKeys = new Dictionary<int, PlayerKeys>();
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

		//4
		keys.Add(new PlayerKeys(
			KeyCode.Keypad8, KeyCode.Keypad6,
			KeyCode.Keypad5, KeyCode.Keypad4,
			KeyCode.Keypad1, KeyCode.Keypad0));
	}

	int lastAssignedKeyIndex;

	public PlayerKeys GetPlayerKeys(int pPlayerNumber)
	{
		PlayerKeys playerKeys;
		if(!assignedKeys.ContainsKey(pPlayerNumber))
		{
			assignedKeys.Add(pPlayerNumber, keys[lastAssignedKeyIndex]);
			lastAssignedKeyIndex++;
		}

		bool res = assignedKeys.TryGetValue(pPlayerNumber, out playerKeys);
		if(!res)
		{
			Debug.LogError($"Keys for player {pPlayerNumber} not configured");
			return keys[0];
		}

		return playerKeys;

		if(pPlayerNumber > keys.Count)
		{
			Debug.LogError($"Keys for player {pPlayerNumber} not configured");
			return GetPlayerKeys(pPlayerNumber % keys.Count);
		}

		return keys[pPlayerNumber - 1];
	}

}
