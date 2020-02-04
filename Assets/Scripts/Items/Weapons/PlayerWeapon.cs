using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerWeapon 
{
	public EWeaponId Id;
	public PlayerWeaponConfig Config;
	public int Ammo;

	public PlayerWeapon(PlayerWeaponConfig pConfig)
	{
		if(pConfig == null)
		{
			Debug.LogError("Config is null");
			return;
		}
		Id = pConfig.Id;
		Config = pConfig;
		Ammo = pConfig.ammo;
	}

	public void Use()
	{
		//Debug.Log($"Use {Id}, Ammo = {Ammo}");
	}

	internal void Add(PlayerWeaponConfig pConfig)
	{
		//Debug.Log($"Add {pConfig.ammo} ammo to {Id}");
		Ammo += pConfig.ammo;
	}
}
