using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerWeapon
{
	public EWeaponId Id;
	//public PlayerWeaponConfig Config;
	public int AmmoLeft;
	public int MagazinesLeft;

	public InHandWeaponInfo Info;
	public InHandWeaponVisualInfo VisualInfo;

	public Player Owner { get; private set; }

	public bool IsRealoading;
	public float RealoadTimeLeft;

	public bool IsActive;

	private bool IsUsed;


	public PlayerWeapon(Player pOwner, EWeaponId pId, InHandWeaponInfo pInHandInfo, InHandWeaponVisualInfo pInHandVisualInfo)
	{
		Id = pId;
		Owner = pOwner;
		AmmoLeft = pInHandInfo.Ammo;
		MagazinesLeft = pInHandInfo.Magazines;

		Info = pInHandInfo;
		VisualInfo = pInHandVisualInfo;
		RealoadTimeLeft = pInHandInfo.Cooldown; //has to be (re)set for UI reloading
	}

	//cant be zero or first use might fail
	public float LastUseTime { get; private set; } = int.MinValue;

	/// <summary>
	/// Tries to use the weapon and reports about the result.
	/// - CantUse: weapon is realoading or is disabled (?)
	/// - Remove: after this use there are no magazines left => weapon should be removed
	/// - Reload: after this use there is no ammo left => weapon should be reloaded
	/// - OK: weapon can be used
	/// </summary>
	public virtual EWeaponUseResult Use()
	{
		if(!CanUse())
			return EWeaponUseResult.CantUse;

		//Debug.Log($"Use {Id}, Ammo = {Ammo}");
		AmmoLeft--;
		if(!IsUsed)
		{
			OnUseStart();
		}

		IsUsed = true;

		if(DebugData.TestInfiniteAmmo)
			AmmoLeft++;


		LastUseTime = Time.time;
		if(AmmoLeft <= 0)
		{
			if(MagazinesLeft <= 0)
			{
				return EWeaponUseResult.Remove;
			}
			return EWeaponUseResult.Reload;
		}

		return EWeaponUseResult.OK;
	}

	private void OnUseStart()
	{
		SoundController.PlayWeaponUseSound(Id, Owner.AudioSource, false);
	}

	public virtual bool CanUse()
	{
		bool isCadencyReady = Time.time > LastUseTime + Info.Cadency;
		return !Owner.Stats.IsDead && 
			!IsRealoading &&
			isCadencyReady;
	}

	public virtual void StopUse()
	{
		IsUsed = false;
	}

	/// <summary>
	/// Adds 1 magazine (1 is spent on reload) and instantly reloads
	/// </summary>
	internal void OnPowerUpAmmo()
	{
		MagazinesLeft += 2;
		Reload();
	}

	internal void OnAddSameWeapon(PlayerWeapon pWeapon)
	{
		//Debug.Log($"Add {pConfig.ammo} ammo to {Id}");
		AmmoLeft += pWeapon.Info.Ammo;
	}

	internal string GetAmmoText()
	{
		if(IsRealoading)
			return RealoadTimeLeft.ToString("0.0");

		return AmmoLeft.ToString();
	}

	internal virtual void OnStartReloadWeapon()
	{
	}

	/// <summary>
	/// Instantly reloads the weapon.
	/// It is expected to have enough magazines.
	/// The proces is controlled by WeponController
	/// </summary>
	internal void Reload()
	{
		//HACK: 666 = infinite magazines. eg. all hero weapons
		if(MagazinesLeft != 666)
			MagazinesLeft--;

		AmmoLeft = Info.Ammo;
		IsRealoading = false;
		RealoadTimeLeft = Info.Cooldown;
		if(IsActive)
		{
			//to update ammo text
			Owner.WeaponController.InvokeWeaponChange(this);
		}
	}	

	/// <summary>
	/// Calculates reload time left, sets it to active weapon,
	/// RealoadTimeLeft value is used to display reload progress.
	/// </summary>
	public void ReportReloadProgress(float pProgress)
	{
		float timeLeft = Info.Cooldown - (Info.Cooldown * pProgress);
		RealoadTimeLeft = timeLeft;
		//Debug.Log($"{this} time left to reload = {timeLeft} | {IsActive}");
	}

	public override string ToString()
	{
		return $"Weapon {Id}";
	}

	public virtual void OnDirectionChange(EDirection pDirection)
	{

	}

	public virtual void OnSetActive()
	{
	}

	
}

public enum EWeaponUseResult
{
	OK, //weapon can be used
	Reload, //out of ammo => triggers reloading
	Remove, //out of magazines => remove weapon
	CantUse //is currently reloading, ...
}
