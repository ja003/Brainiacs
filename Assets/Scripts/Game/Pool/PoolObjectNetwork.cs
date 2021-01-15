using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool object instanced over the network
/// </summary>
public abstract class PoolObjectNetwork : GameBehaviour, IPoolObject
{
	private PoolObject _poolObject;
	protected PoolObject poolObject
	{
		get
		{
			if(_poolObject == null)
				_poolObject = GetComponent<PoolObject>();
			return _poolObject;
		}
	}

	protected PoolObjectPhoton _photon;
	public PoolObjectPhoton Photon
	{
		get
		{
			if(_photon == null)
				_photon = GetComponent<PoolObjectPhoton>();
			return _photon;
		}
	}

	//sends OnSetActive state over network
	[SerializeField] bool sendActivateMsg = true;
	//should be false for objects that are destroyed using PhotonNetwork.Destroy (eg. Projectiles)
	[SerializeField] bool sendDeactivateMsg = true;

	/// <summary>
	/// Only the owner of the object can return it to pool.
	/// If not mine => deactivate object (so it cant be interacted with)
	/// and send request to return in to the pool.
	/// </summary>
	public void ReturnToPool(bool pSendRPC = true)
	{
		if(Photon.IsMine)
		{
			//Debug.Log($"{gameObject.name} ReturnToPool");
			poolObject.ReturnToPool();
		}
		else if(pSendRPC)
		{
			//Debug.Log($"{gameObject.name} send Pool_ReturnToPool");
			SetActive(false);
			Photon.Send(EPhotonMsg.Pool_ReturnToPool, Photon.view.Owner);
		}
	}

	public void OnReturnToPool()
	{
		OnReturnToPool2();
		Photon.OnReturnToPool();
	}

	protected abstract void OnReturnToPool2();

	public void OnSetActive(bool pValue)
	{
		OnSetActive0(pValue);

		if((pValue && sendActivateMsg) || (!pValue && sendDeactivateMsg))
			Photon.Send(EPhotonMsg.Pool_SetActive, pValue);
	}

	protected abstract void OnSetActive0(bool pValue);

	public void SetActive(bool pValue)
	{
		poolObject?.SetActive(pValue);
	}

	public bool IsMine()
	{
		return Photon.IsMine;
	}
}
