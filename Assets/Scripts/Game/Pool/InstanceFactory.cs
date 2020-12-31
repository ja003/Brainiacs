using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles instantiating of game objects.
/// Might be extended for transform parent => no need, handled by PoolManager
/// </summary>
public static class InstanceFactory
{
	public static bool poolInitialized;

	public static GameObject Instantiate(GameObject pPrefab, Vector2 pPosition, bool pNetworkInstance = true)
	{
		if(pPrefab == null)
		{
			Debug.LogError("Instantiate null");
			return null;
		}
		if(!poolInitialized)
		{
			Game.Instance.Pool.Init();
		}

		bool isMultiplayer = Brainiacs.Instance.GameInitInfo.IsMultiplayer();

		GameObject instance;
		//NOTE: we could use PhotonNetwork.Instantiate always but a lot of warnings are thrown
		//=> call PoolManager.Instantiate / PoolManager.Destroy directly

		if(pNetworkInstance && PhotonNetwork.IsConnected && isMultiplayer)// || debug.Players)
			instance = PhotonNetwork.Instantiate(pPrefab.name, pPosition, Quaternion.identity);
		else
		{
			if(isMultiplayer && pNetworkInstance)
				Debug.LogError("Not conected to server");
			instance = Game.Instance.Pool.Instantiate(pPrefab.name, pPosition, Quaternion.identity);
		}

		return instance;
	}

	public static GameObject Instantiate(GameObject pPrefab)//, bool pNetworkInstance = true)
	{
		bool isNetworkInstance = pPrefab.GetComponent<PhotonView>();
		return Instantiate(pPrefab, Vector2.zero, isNetworkInstance);
	}

	internal static void Destroy(GameObject pGameObject)//, bool pNetworkInstance = true)
	{
		bool isNetworkInstance = pGameObject.GetComponent<PhotonView>();

		if(isNetworkInstance && PhotonNetwork.IsConnected)
			PhotonNetwork.Destroy(pGameObject);
		else
		{
			if(isNetworkInstance && Brainiacs.Instance.GameInitInfo.IsMultiplayer())
				Debug.LogError("Not conected to server");
			Game.Instance.Pool.Destroy(pGameObject);
		}
	}
}
