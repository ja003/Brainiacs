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

	private float lastUseTime;
	public virtual EWeaponUseResult Use()
	{
		if(IsRealoading)
			return EWeaponUseResult.CantUse;

		if(Time.time < lastUseTime + Config.Cadency)
		{
			return EWeaponUseResult.CantUse;
		}

		//Debug.Log($"Use {Id}, Ammo = {Ammo}");
		if(Ammo <= 0)
		{
			if(magazines <= 0)
			{
				return EWeaponUseResult.Remove;
			}
			return EWeaponUseResult.Reload;
		}
		Ammo--;
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
		magazines += 2;
		Reload();
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
