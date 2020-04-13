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
    public static GameObject Instantiate(GameObject pPrefab, Vector3 pPosition)
    {
		GameObject instance;
		if(PhotonNetwork.IsConnected)
			instance = PhotonNetwork.Instantiate(pPrefab.name, pPosition, Quaternion.identity);
		else
		{
			if(Brainiacs.Instance.GameInitInfo.IsMultiplayer())
				Debug.LogError("Not conected to server");
			instance = Brainiacs.Instantiate(pPrefab, pPosition, Quaternion.identity);
		}
		return instance;
	}

	public static GameObject Instantiate(GameObject pPrefab)
	{
		return Instantiate(pPrefab, Vector3.zero);
	}
}
