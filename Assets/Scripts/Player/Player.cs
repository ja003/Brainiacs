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

	[SerializeField]
	public PlayerHealth Health;

	public BoxCollider2D Collider => boxCollider2D;

	public PlayerItemHandler ItemHandler;
	public PlayerStats Stats { get; private set; }

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			Health.DebugDie();
		}
	}


	internal void SetInfo(PlayerInitInfo pPlayerInfo, Vector3 pSpawnPosition)
	{
		Stats = new PlayerStats(pPlayerInfo);
		ItemHandler = new PlayerItemHandler(Stats, WeaponController);
		Health.Init(Stats);

		input.Keys = pPlayerInfo.PlayerKeys;


		WeaponController.AddWeapon(
			brainiacs.ItemManager.GetHeroWeaponConfig(pPlayerInfo.Hero));
		

		//todo: delete. test adding weapons
		WeaponController.AddWeapon(
			brainiacs.ItemManager.GetPlayerWeaponConfig(EWeaponId.TestGun2));
		//todo: delete. test adding same weapon
		WeaponController.AddWeapon(
			brainiacs.ItemManager.GetPlayerWeaponConfig(EWeaponId.TestGun2));


		//SPECIAL
		//last one is active
		WeaponController.AddWeapon(
			brainiacs.ItemManager.GetHeroSpecialWeaponConfig(pPlayerInfo.Hero));


		Visual.Init(spriteRend, brainiacs.HeroManager.GetHeroConfig(pPlayerInfo.Hero));

		Movement.SpawnAt(pSpawnPosition);

	}

	
}
