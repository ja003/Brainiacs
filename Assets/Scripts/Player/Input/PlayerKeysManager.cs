using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeysManager : BrainiacsBehaviour
{
	Dictionary<EKeyset, PlayerKeys> keys = new Dictionary<EKeyset, PlayerKeys>();

	internal void UpdateKeySet(EKeyset pSet, PlayerKeys pPlayerKeys)
	{
		if(!keys.ContainsKey(pSet))
			keys.Add(pSet, pPlayerKeys);
		else
			keys[pSet] = pPlayerKeys;

		brainiacs.PlayerPrefs.SetPlayerKeys(pSet, pPlayerKeys);
	}

	public PlayerKeys GetPlayerKeys(EKeyset pSet)
	{
		if(!keys.ContainsKey(pSet))
		{
			if(Time.time > 1)
				Debug.LogError($"Set {pSet} not defined");
			return new PlayerKeys(pSet);
		}
		return keys[pSet];
	}
}

