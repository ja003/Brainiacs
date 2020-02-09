using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerWeapon
{
	public EWeaponId Id;
	public PlayerWeaponConfig Config;
	public int Ammo;
	public int magazines;

	protected Player owner;

	public bool IsRealoading;
	public float RealoadTimeLeft;

	public bool IsActive;

	public PlayerWeapon(PlayerWeaponConfig pConfig, Player pOwner)
	{
		if(pConfig == null)
		{
			Debug.LogError("Config is null");
			return;
		}
		Id = pConfig.Id;
		Config = pConfig;
		Ammo = pConfig.ammo;
		magazines = pConfig.Magazines;
		owner = pOwner;
	}

	public virtual EWeaponUseResult Use()
	{
		if(IsRealoading)
			return EWeaponUseResult.CantUse;
		//Debug.Log($"Use {Id}, Ammo = {Ammo}");
		Ammo--;
		if(Ammo <= 0)
		{
			if(magazines <= 0)
			{
				return EWeaponUseResult.Remove;
			}
			return EWeaponUseResult.Reload;
		}
		return EWeaponUseResult.OK;
	}


	internal void Add(PlayerWeaponConfig pConfig)
	{
		//Debug.Log($"Add {pConfig.ammo} ammo to {Id}");
		Ammo += pConfig.ammo;
	}

	internal string GetAmmoText()
	{
		if(IsRealoading)
			return RealoadTimeLeft.ToString("0.0");

		return Ammo.ToString();
	}

	/// <summary>
	/// Instantly reloads the weapon.
	/// It is expected to have enough magazines.
	/// The proces is controlled by WeponController
	/// </summary>
	internal void Reload()
	{
		magazines--;
		Ammo = Config.ammo;
		IsRealoading = false;
		if(IsActive)
		{
			//to update ammo text
			owner.WeaponController.InvokeWeaponChange(this);
		}
	}

	/// <summary>
	/// Calculates reload time left, sets it to active weapon and
	/// invokes info change (UI)
	/// </summary>
	/// <param name="pProgress">range: 0 - 1</param>
	public void ReportReloadProgress(float pProgress)
	{
		float timeLeft = Config.Cooldown - (Config.Cooldown * pProgress);
		RealoadTimeLeft = timeLeft;
		//Debug.Log($"{this} time left to reload = {timeLeft} | {IsActive}");
		if(IsActive)
			owner.WeaponController.InvokeWeaponChange(this);
	}

	public override string ToString()
	{
		return $"Weapon {Id}";
	}
}

public enum EWeaponUseResult
{
	OK, //weapon can be used
	Reload, //out of ammo => triggers reloading
	Remove, //out of magazines => remove weapon
	CantUse //is currently reloading, ...
}
