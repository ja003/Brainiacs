/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
using UnityEngine;
using System.Collections;
using Photon.Pun;
using FlatBuffers;

//todo: some classes (MapObstackle, ..) shouldnt need view
//[RequireComponent(typeof(PhotonView))]
public abstract class PoolObject : GameBehaviour, IPunInstantiateMagicCallback
{
	[Header("Pool")]
	public string poolName;
	//defines whether the object is waiting in pool or is in use
	public bool isPooled;

	[SerializeField] public PoolObjectPhoton Photon;

	//protected Game game => Game.Instance;

	//[SerializeField] public bool EnableOnInstanced; //true => will be auto enabled after being drwan from pool
	//[SerializeField] public bool DisableOnDestroy; //true => will be auto disabled after return to pool

	public bool IsPhotonInstantiated;
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		IsPhotonInstantiated = true;
		//Debug.Log("OnPhotonInstantiate " + gameObject.name);
		SetActive(false);
	}

	//TODO: implement OnEnable/OnDisable and call SetActive?

	/// <summary>
	/// IMPORTANT: use this method instead of gameObject.SetActive
	/// Object state has to be shared across network
	/// </summary>
	public void SetActive(bool pValue)
	{
		gameObject.SetActive(pValue);
		OnSetActive(pValue);
		Photon?.Send(EPhotonMsg.Pool_SetActive, pValue);
		//if(view.ViewID != 0 && view.IsMine)
		//{
		//	Send(EPhotonMsg.Pool_SetActive, pValue);
		//}
	}

	protected abstract void OnSetActive(bool pValue);

	//sealed protected override bool CanSend(EPhotonMsg pMsgType)
	//{
	//	switch(pMsgType)
	//	{
	//		case EPhotonMsg.Pool_SetActive:
	//			return view.IsMine;
	//	}

	//	//return true;
	//	return CanSendMsg(pMsgType);
	//}

	//protected abstract bool CanSendMsg(EPhotonMsg pMsgType);

	//protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	//{
	//	switch(pReceivedMsg)
	//	{
	//		case EPhotonMsg.Pool_SetActive:
	//			bool value = (bool)pParams[0];
	//			SetActive(value);
	//			return;
	//	}
	//}

	//protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	//{
		
	//}

}
