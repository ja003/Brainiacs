using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PlayerWeaponSpecialController : PoolObject, IPunOwnershipCallbacks
{
	//private SpecialWeaponPhoton _photon;
	//public SpecialWeaponPhoton Photon
	//{
	//	get
	//	{
	//		if(_photon == null)
	//			_photon = GetComponent<SpecialWeaponPhoton>();
	//		return _photon;
	//	}
	//}

	protected PlayerWeaponController weaponContoller;
	public Player Owner { get; private set; }
	public void debug_AssignOwner(Player pOwner)
	{
		Owner = pOwner;
	}

	protected bool isInited;
	public void Init(Player pOwner)
	{
		Owner = pOwner;
		weaponContoller = pOwner.WeaponController;
		if(GetCollider() != null)
			Physics2D.IgnoreCollision(GetCollider(), pOwner.Collider);
		//Debug.Log($"Ignore collisions between {boxCollider2D.gameObject.name} and {owner}");
		gameObject.name += $"({pOwner.InitInfo.Name})";
		isInited = true;

		OnInit();
		//SetActive(false);

		Photon.Send(EPhotonMsg.Special_Init, pOwner.InitInfo.Number);
	}

	//TODO: dont know how to use
	public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
	{
		Debug.Log($"OnOwnershipRequest {targetView}, {requestingPlayer}");
	}

	public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
	{
		Debug.Log($"OnOwnershipTransfered {targetView}, {previousOwner}");
	}

	internal virtual bool CanUse()
	{
		return isInited;
	}

	/// <summary>
	/// Weapon key pressed
	/// </summary>
	public void Use()
	{
		Photon.Send(EPhotonMsg.Special_Use);
		SetActive(true); //todo: check is it needed?
		OnUse();
	}
	protected abstract void OnUse();

	/// <summary>
	/// Weapon key released
	/// </summary>
	public void StopUse() {
		Photon.Send(EPhotonMsg.Special_StopUse);
		OnStopUse();
	}
	protected abstract void OnStopUse();

	//current player direction
	protected EDirection playerDirection => Owner.Movement.CurrentDirection;



	//DEBUG
	//local remote assigned to this object
	public PlayerWeaponSpecialController _LocalImage { get; internal set; }
	//the object this object is assigned to
	public PlayerWeaponSpecialController _RemoteOwner { get; internal set; }

	internal virtual void OnStartReloadWeapon()
	{
	}

	//public abstract PhotonMessenger GetLocalImageNetwork();



	protected abstract void OnInit();

	protected virtual Collider2D GetCollider()
	{
		return boxCollider2D;
	}

	internal virtual void OnDirectionChange(EDirection pDirection)
	{
	}

	internal virtual void OnSetActive()
	{
	}


	/// PHOTON

	//protected override bool CanSendMsg(EPhotonMsg pMsgType)
	//{
	//	return view.IsMine || _LocalImage;
	//}

	//protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	//{
	//	base.HandleMsg(pReceivedMsg, pParams, bb);
	//	switch(pReceivedMsg)
	//	{
	//		case EPhotonMsg.Special_Init:
	//			int playerNumber = (int)pParams[0];
	//			Game.Instance.PlayerManager.OnAllPlayersAdded.AddAction(() => InitController(playerNumber));
	//			break;

	//		case EPhotonMsg.Special_Use:
	//			Use();
	//			break;
	//		case EPhotonMsg.Special_StopUse:
	//			StopUse();
	//			break;
	//	}
	//}

	//private void InitController(int pPlayerNumber)
	//{
	//	if(DEBUG_LOG)
	//		Debug.Log($"P({pPlayerNumber}) InitController {name}");
	//	Player player = Game.Instance.PlayerManager.GetPlayer(pPlayerNumber);
	//	Init(player);
	//}

	//protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	//{
	//	if(_LocalImage)
	//	{
	//		_LocalImage.HandleMsg(pMsgType, pParams);
	//	}
	//}


}
