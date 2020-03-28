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

	protected Player owner;

	public bool IsRealoading;
	public float RealoadTimeLeft;

	public bool IsActive;

	public PlayerWeapon(Player pOwner, EWeaponId pId, InHandWeaponInfo pInHandInfo, InHandWeaponVisualInfo pInHandVisualInfo)
	{
		Id = pId;
		owner = pOwner;
		AmmoLeft = pInHandInfo.Ammo;
		MagazinesLeft = pInHandInfo.Magazines;

		Info = pInHandInfo;
		VisualInfo = pInHandVisualInfo;
		RealoadTimeLeft = pInHandInfo.Cooldown; //has to be (re)set for UI reloading
	}

	//cant be zero or first use might fail
	private float lastUseTime = int.MinValue;
	public virtual EWeaponUseResult Use()
	{
		if(IsRealoading)
			return EWeaponUseResult.CantUse;

		if(Time.time < lastUseTime + Info.Cadency)
		{
			return EWeaponUseResult.CantUse;
		}

		//Debug.Log($"Use {Id}, Ammo = {Ammo}");
		if(AmmoLeft <= 0)
		{
			if(MagazinesLeft <= 0)
			{
				return EWeaponUseResult.Remove;
			}
			return EWeaponUseResult.Reload;
		}
		AmmoLeft--;
		lastUseTime = Time.time;
		return EWeaponUseResult.OK;
	}

	public virtual void StopUse()
	{

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

	/// <summary>
	/// Instantly reloads the weapon.
	/// It is expected to have enough magazines.
	/// The proces is controlled by WeponController
	/// </summary>
	internal void Reload()
	{
		MagazinesLeft--;
		AmmoLeft = Info.Ammo;
		IsRealoading = false;
		RealoadTimeLeft = Info.Cooldown;
		if(IsActive)
		{
			//to update ammo text
			owner.WeaponController.InvokeWeaponChange(this);
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
