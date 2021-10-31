﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerWeapon
{
	public EWeaponId Id;
	//public PlayerWeaponConfig Config;
	public int AmmoLeft;
	public int MagazinesLeft;
	private float cadency;

	public InHandWeaponInfo Info;
	public InHandWeaponVisualInfo VisualInfo;

	public Player Owner { get; private set; }

	public bool IsRealoading;
	public float RealoadTimeLeft;

	public bool IsActive;

	public bool IsUsed { get; private set; }

	public PlayerWeapon(Player pOwner, EWeaponId pId, InHandWeaponInfo pInHandInfo, InHandWeaponVisualInfo pInHandVisualInfo)
	{
		Id = pId;
		Owner = pOwner;
		AmmoLeft = pInHandInfo.Ammo;
		MagazinesLeft = pInHandInfo.Magazines - 1;
		cadency = pInHandInfo.Cadency;

		Info = pInHandInfo;
		VisualInfo = pInHandVisualInfo;
		RealoadTimeLeft = pInHandInfo.Cooldown; //has to be (re)set for UI reloading
	}

	//cant be zero or first use might fail
	public float LastUseTime { get; private set; } = int.MinValue;
	public float LastUseStartTime { get; private set; } = int.MinValue;

	/// <summary>
	/// Tries to use the weapon and reports about the result.
	/// - CantUse: weapon is reloading or is disabled (?)
	/// - Remove: after this use there are no magazines left => weapon should be removed
	/// - Reload: after this use there is no ammo left => weapon should be reloaded
	/// - OK: weapon can be used
	/// </summary>
	public virtual EWeaponUseResult Use()
	{
		if(!CanUse())
			return EWeaponUseResult.CantUse;

		//Debug.Log($"Use {Id}, Ammo = {Ammo}");
		//AmmoLeft--; //incorrect for 0-cadency weapons => handled in OnUseStart and OnKeepUse
		if(IsUsed)
			OnKeepUse();
		else
			OnUseStart();

		IsUsed = true;

		if(AmmoLeft == 0 && CDebug.Instance.InfiniteAmmo)
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
	private void OnKeepUse()
	{
		if(!is0cadency)
			return;
		//if weapon is 0-cadency => reduce ammo every second
		if(Time.time - (wasAmmoReduced ? lastAmmoReduceTime : LastUseStartTime) + usageDuration > 1)
		{
			//Debug.Log($"{Id} OnKeepUse - Reduce ammo. {usageDuration}, {LastUseStartTime}");
			usageDuration = 0;
			AmmoLeft--;
			lastAmmoReduceTime = Time.time;
			wasAmmoReduced = true;
		}
	}

	//info for 0-cadency weapons
	bool is0cadency => cadency < 0.01f;
	float lastAmmoReduceTime;
	bool wasAmmoReduced;
	float usageDuration;

	private void OnUseStart()
	{
		//Debug.Log("OnUseStart");
		LastUseStartTime = Time.time;
		if(!is0cadency)
			AmmoLeft--;

		wasAmmoReduced = false;
		Owner.WeaponController.PlayWeaponUseSound(Id);
	}

	public virtual void StopUse(bool pIsUserInput)
	{
		//if weapon wasnt stopped by player input => it wasnt being used
		if(pIsUserInput)
		{
			//store usageDuration for next use
			usageDuration += Time.time - (wasAmmoReduced ? lastAmmoReduceTime : LastUseStartTime);
			//Debug.Log(Id + " usageDuration = " + usageDuration);
		}
		//Debug.Log("StopUse");
		IsUsed = false;
	}

	public virtual bool CanUse()
	{
		bool isCadencyReady = Time.time > LastUseTime + Info.Cadency;
		bool isActiveLongEnough = Time.time - setActiveTime > 0.15f; 
		return 
			!Owner.Stats.IsDead &&
			!IsRealoading &&
			isCadencyReady &&
			isActiveLongEnough;
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
	/// The process is controlled by WeponController
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

	float setActiveTime;

	public virtual void OnSetActive()
	{
		setActiveTime = Time.time;
	}


}

public enum EWeaponUseResult
{
	OK, //weapon can be used
	Reload, //out of ammo => triggers reloading
	Remove, //out of magazines => remove weapon
	CantUse //is currently reloading, ...
}
