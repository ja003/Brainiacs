using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles instantiating of game objects.
/// Might be extended for transform parent
/// </summary>
public class InstanceFactory
{
    public static GameObject Instantiate(GameObject pPrefab)
    {
		GameObject instance;
		if(PhotonNetwork.IsConnected)
			instance = PhotonNetwork.Instantiate(pPrefab.name, Vector3.zero, Quaternion.identity);
		else
		{
			if(Brainiacs.Instance.GameInitInfo.IsMultiplayer())
				Debug.LogError("Not conected to server");
			instance = Brainiacs.Instantiate(pPrefab, Vector3.zero, Quaternion.identity);
		}
		return instance;
	}
}
