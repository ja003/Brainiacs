using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
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
		if(!poolObject.IsPhotonInstantiated)
			return false;

		switch(pMsgType)
		{
			case EPhotonMsg.Pool_SetActive:
				return view.IsMine;
		}

		return CanSendMsg(pMsgType);
		//return true;
	}

	protected abstract bool CanSendMsg(EPhotonMsg pMsgType);

	sealed protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Pool_SetActive:
				bool value = (bool)pParams[0];
				poolObject.SetActive(value);
				return;

			//default:
			//	Debug.LogError(this.gameObject.name + " message not handled " + pReceivedMsg);
			//	break;
		}

		HandleMsg2(pReceivedMsg, pParams, bb);
	}

	protected abstract void HandleMsg2(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb);


	protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{

	}
}
