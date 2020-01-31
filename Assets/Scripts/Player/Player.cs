using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GameBehaviour
{
	
	[SerializeField]
	private PlayerInput input;

	[SerializeField]
	private PlayerWeaponController weaponController;

	public PlayerItemHandler ItemHandler;
	public PlayerStats stats;

	internal void SetInfo(PlayerInitInfo pPlayerInfo)
	{
		stats = new PlayerStats(pPlayerInfo);
		ItemHandler = new PlayerItemHandler(stats, weaponController);

		input.Keys = pPlayerInfo.PlayerKeys;

		weaponController.AddWeapon(
			brainiacs.ItemManager.GetHeroWeaponConfig(pPlayerInfo.Hero));

		//todo: delete. test adding weapons
		weaponController.AddWeapon(
			brainiacs.ItemManager.GetPlayerWeaponConfig(EWeaponId.TestGun2));
		//todo: delete. test adding same weapon
		weaponController.AddWeapon(
			brainiacs.ItemManager.GetPlayerWeaponConfig(EWeaponId.TestGun2));

	}
}
