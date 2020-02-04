using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GameBehaviour
{

	[SerializeField]
	private PlayerInput input;

	[SerializeField]
	public PlayerWeaponController WeaponController;

	[SerializeField]
	public PlayerVisual Visual;

	[SerializeField]
	public PlayerMovement Movement;

	public PlayerItemHandler ItemHandler;
	public PlayerStats stats;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			DebugDie();
		}
	}

	private void DebugDie()
	{
		Visual.Die();
	}

	internal void SetInfo(PlayerInitInfo pPlayerInfo)
	{
		stats = new PlayerStats(pPlayerInfo);
		ItemHandler = new PlayerItemHandler(stats, WeaponController);

		input.Keys = pPlayerInfo.PlayerKeys;

		WeaponController.AddWeapon(
			brainiacs.ItemManager.GetHeroWeaponConfig(pPlayerInfo.Hero));

		//todo: delete. test adding weapons
		WeaponController.AddWeapon(
			brainiacs.ItemManager.GetPlayerWeaponConfig(EWeaponId.TestGun2));
		//todo: delete. test adding same weapon
		WeaponController.AddWeapon(
			brainiacs.ItemManager.GetPlayerWeaponConfig(EWeaponId.TestGun2));

		Visual.Init(spriteRend, brainiacs.HeroManager.GetHeroConfig(pPlayerInfo.Hero));

		Movement.Init();

	}

	
}
