using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles instantiating of game objects.
/// Might be extended for transform parent => no need, handled by PoolManager
/// </summary>
public class InstanceFactory
{
	public static GameObject Instantiate(GameObject pPrefab, Vector3 pPosition)
	{
		if(pPrefab == null)
		{
			Debug.LogError("Instantiate null");
			return null;
		}
		bool isMultiplayer = Brainiacs.Instance.GameInitInfo.IsMultiplayer();

		GameObject instance;
		//NOTE: we could use PhotonNetwork.Instantiate always but a lot of warnings are thrown
		//=> call PoolManager.Instantiate / PoolManager.Destroy directly

		if(PhotonNetwork.IsConnected && isMultiplayer)// || DebugData.TestPlayers)
			instance = PhotonNetwork.Instantiate(pPrefab.name, pPosition, Quaternion.identity);
		else
		{
			if(isMultiplayer)
				Debug.LogError("Not conected to server");
			instance = Game.Instance.Pool.Instantiate(pPrefab.name, pPosition, Quaternion.identity);
		}

		return instance;
	}

	public static GameObject Instantiate(GameObject pPrefab)
	{
		return Instantiate(pPrefab, Vector3.zero);
	}

	internal static void Destroy(GameObject pGameObject)
	{
		if(PhotonNetwork.IsConnected)
			PhotonNetwork.Destroy(pGameObject);
		else
		{
			if(Brainiacs.Instance.GameInitInfo.IsMultiplayer())
				Debug.LogError("Not conected to server");
			Game.Instance.Pool.Destroy(pGameObject);
		}
	}
}
