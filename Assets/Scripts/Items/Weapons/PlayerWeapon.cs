using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerWeapon 
{
	public EWeaponId Id;
	public PlayerWeaponConfig Config;
	public int Ammo;

	protected Player owner;

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
		owner = pOwner;
	}

	public virtual void Use()
	{
		//Debug.Log($"Use {Id}, Ammo = {Ammo}");
		Ammo--;
	}

	internal void Add(PlayerWeaponConfig pConfig)
	{
		//Debug.Log($"Add {pConfig.ammo} ammo to {Id}");
		Ammo += pConfig.ammo;
	}
}
