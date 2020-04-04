using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeaponSpecialController : GameBehaviour, IPunOwnershipCallbacks
{
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
		gameObject.SetActive(false);

		Network.Send(EPhotonMsg.Special_Init, pOwner.InitInfo.Number);
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
		Network.Send(EPhotonMsg.Special_Use);
		gameObject.SetActive(true);
		OnUse();
	}
	protected abstract void OnUse();

	/// <summary>
	/// Weapon key released
	/// </summary>
	public void StopUse() {
		Network.Send(EPhotonMsg.Special_StopUse);
		OnStopUse();
	}
	protected abstract void OnStopUse();

	//current player direction
	protected EDirection playerDirection => Owner.Movement.CurrentDirection;



	//DEBUG
	//local remote assigned to this object
	public PlayerWeaponSpecialController _LocalRemote { get; internal set; }
	//the object this object is assigned to
	public PlayerWeaponSpecialController _RemoteOwner { get; internal set; }

	[SerializeField] public SpecialWeaponNetwork Network;

	internal virtual void OnStartReloadWeapon()
	{
	}

	//public abstract PhotonMessenger GetLocalRemoteNetwork();



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

	
}
