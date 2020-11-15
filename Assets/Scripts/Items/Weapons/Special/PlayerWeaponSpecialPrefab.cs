using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PlayerWeaponSpecialPrefab : PoolObjectNetwork
{
	[SerializeField] protected float pushForce;
	public float PushForce => pushForce;

	protected Player owner { get; private set; }
	public void debug_AssignOwner(Player pOwner)
	{
		owner = pOwner;
	}

	public new PlayerWeaponSpecialPrefabPhoton Photon => (PlayerWeaponSpecialPrefabPhoton)base.Photon;

	protected bool isInited;

	//new instance of prefab will be pooled on every use
	//TRUE - bomb, truck, mine, ...
	//FALSE - flame, daVinci, ...
	[SerializeField] public bool InstanceOnEveryUse;

	/// <summary>
	/// Used as self init for remote object
	/// </summary>
	protected override void OnSetActive0(bool pValue)
	{
		if(pValue && !isInited && isMultiplayer)
		{
			//Debug.Log(gameObject.name + " OnPhotonInstantiated " + Photon.view.Owner);
			//all players has to be added
			game.PlayerManager.OnAllPlayersAdded.AddAction(() =>
			{
				Player owner = game.PlayerManager.GetPlayer(Photon.view.Owner);
				if(owner == null)
				{
					Debug.LogError(gameObject.name + " Owner not found");
					return;
				}
				Init(owner);
			});
		}
		OnSetActive2(pValue);
	}


	/// <summary>
	/// Called after each pool.
	/// TODO: maybe not neccessary to init multiple time, since we always pool 
	/// object that is mine (new int EasyObjectPool)
	/// New owner has to be always assigned because prefab could have been 
	/// previously instantiated by another player.
	/// </summary>
	public void Init(Player pOwner)
	{
		owner = pOwner;
		if(GetCollider() != null)
			Physics2D.IgnoreCollision(GetCollider(), pOwner.Collider);

		//Debug.Log($"Ignore collisions between {GetCollider().gameObject.name} and {owner}");
		isInited = true;

		OnInit();

		DoInTime(debug_AssignOwnerName, 1); //has to be called after prefab is instanced
											//SetActive(false);

		//replaced by OnPhotonInstantiated
		//Photon.Send(EPhotonMsg.Special_Init, pOwner.InitInfo.Number);
	}

	

	protected abstract void OnSetActive2(bool pValue);

	//protected override void OnPhotonInstantiated() { }

	public virtual void OnStartReloadWeapon()
	{
	}

	protected override void OnReturnToPool2()
	{
		//Debug.Log(gameObject.name + " OnReturnToPool");
		//prefab has to be initialized after each pool
		isInited = false;
		OnReturnToPool3();
	}

	protected abstract void OnReturnToPool3();

	//Photon.view not assigned yet
	//public override void OnInstantiated()
	//{
	//	Debug.Log(gameObject.name + " OnInstantiated " + Photon.view.Owner);
	//}

	string origName = "";
	private void debug_AssignOwnerName()
	{
		if(origName.Length == 0)
			origName = gameObject.name;

		//just debug
		origName += $"({owner.InitInfo.Name})";
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
		if(!CanUse())
			return;

		//Photon.Send(EPhotonMsg.Special_Use);
		//SetActive(true); //todo: check is it needed?
		OnUse();
	}
	protected abstract void OnUse();

	/// <summary>
	/// Weapon key released
	/// </summary>
	public void StopUse()
	{
		//Photon.Send(EPhotonMsg.Special_StopUse);
		OnStopUse();
	}
	protected abstract void OnStopUse();

	//current player direction
	protected EDirection playerDirection => owner.Movement.CurrentDirection;



	protected abstract void OnInit();

	protected virtual Collider2D GetCollider()
	{
		return boxCollider2D;
	}

	protected Vector2 GetPush(Transform pTarget)
	{
		return (pTarget.position - transform.position).normalized * pushForce;
	}
}
