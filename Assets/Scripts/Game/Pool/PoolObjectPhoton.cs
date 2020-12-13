using FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PoolObject))]
public abstract class PoolObjectPhoton : PhotonMessenger
{
	private PoolObject _poolObject = null;
	public PoolObject poolObject
	{
		get
		{
			if(_poolObject == null)
				_poolObject = GetComponent<PoolObject>();
			return _poolObject;
		}
	}

	sealed protected override bool CanSend(EPhotonMsg pMsgType)
	{
		if(!poolObject.IsPhotonInstantiated && isMultiplayer)
			return false;

		if(view.ViewID == 0 && isMultiplayer)
		{
			if(pMsgType == EPhotonMsg.Pool_SetActive)
				Debug.LogWarning("Cant send message yet. " + pMsgType);
			else
				Debug.LogError("Cant send message yet. " + pMsgType);

			return false;
		}

		switch(pMsgType)
		{
			//both sides can request return object to pool
			case EPhotonMsg.Pool_ReturnToPool:
				return true;
			case EPhotonMsg.Pool_SetActive:
				return IsMine;
		}

		return CanSend2(pMsgType);
		//return true;
	}

	protected abstract bool CanSend2(EPhotonMsg pMsgType);

	sealed protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Pool_SetActive:
				bool value = (bool)pParams[0];
				poolObject.SetActive(value);
				return;

			case EPhotonMsg.Pool_ReturnToPool:
				poolObject.ReturnToPool();
				return;

			//default:
			//	Debug.LogError(this.gameObject.name + " message not handled " + pReceivedMsg);
			//	break;
		}

		HandleMsg2(pReceivedMsg, pParams, bb);
	}

	protected abstract void HandleMsg2(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb);


	protected override void debug_SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{

	}

	public virtual void OnReturnToPool() { }
}
