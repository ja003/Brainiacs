/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
using UnityEngine;
using System.Collections;
using Photon.Pun;
using FlatBuffers;
using System;

/// <summary>
/// Script attached to any poolable object.
/// If there is a script with interface IPoolObject it passes all calls to it.
/// </summary>
public class PoolObject : GameBehaviour, IPunInstantiateMagicCallback
{
	[Header("Pool")]
	public string poolName;
	//defines whether the object is waiting in pool or is in use
	public bool isPooled;

	//[SerializeField] public PoolObjectPhoton Photon;

	//protected Game game => Game.Instance;

	//[SerializeField] public bool EnableOnInstanced; //true => will be auto enabled after being drwan from pool
	//[SerializeField] public bool DisableOnDestroy; //true => will be auto disabled after return to pool

	IPoolObject poolObject;
	private new void Awake()
	{
		poolObject = GetComponent<IPoolObject>();
		base.Awake();
	}


	public bool IsMine()
	{
		if(poolObject != null)
			return poolObject.IsMine();
		return true;
	}

	public bool IsPhotonInstantiated;
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		IsPhotonInstantiated = true;
		//Debug.Log("OnPhotonInstantiate " + gameObject.name);
		//SetActive(false);
		//OnPhotonInstantiated();
	}

	//protected abstract void OnPhotonInstantiated();

	/// <summary>
	/// Called when instance is created
	/// </summary>
	public virtual void OnInstantiated()
	{
		//...will see if needed
		//Debug.Log($"{gameObject.name} OnInstantiated");
	}

	public void OnGetFromPool()
	{
		//some objects (speacial weapons prefabs, ...) cant be activated from start
		//SetActive(true);
	}

	public bool IsNetworkObject()
	{
		return poolObject != null;
	}

	//{
	//	Debug.Log("OnInstantiated " + gameObject.name);
	//}

	//TODO: implement OnEnable/OnDisable and call SetActive?

	/// <summary>
	/// IMPORTANT: use this method instead of gameObject.SetActive
	/// Object state has to be shared across network
	/// </summary>
	public void SetActive(bool pValue)
	{
		gameObject.SetActive(pValue);
		poolObject?.OnSetActive(pValue);
		//OnSetActive(pValue);
		//Photon?.Send(EPhotonMsg.Pool_SetActive, pValue);
		//if(view.ViewID != 0 && view.IsMine)
		//{
		//	Send(EPhotonMsg.Pool_SetActive, pValue);
		//}
	}

	//protected abstract void OnSetActive(bool pValue);

	public void ReturnToPool()
	{
		if(isPooled)
		{
			//this happens eg. when Projectile hits 2 items (when there are 2 stuck on each other)
			//and projectile hit is called twice.
			return;
		}

		//if(Photon.IsMine)
		if(IsMine())
			InstanceFactory.Destroy(gameObject);
	}

	public void OnReturnToPool()
	{
		//Photon.OnReturnToPool();
		poolObject?.OnReturnToPool();
		//OnReturnToPool2();
	}


	//protected abstract void OnReturnToPool2();
}
