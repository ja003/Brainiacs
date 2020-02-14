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

	[SerializeField]
	public PlayerItemController ItemController;

	[SerializeField]
	public PlayerStats Stats;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			Health.DebugDie();
		}
	}


	internal void SetInfo(PlayerInitInfo pPlayerInfo, Vector3 pSpawnPosition)
	{
		Stats.Init(pPlayerInfo);

		input.Keys = pPlayerInfo.PlayerKeys;


		WeaponController.AddWeapon(
			brainiacs.ItemManager.GetHeroWeaponConfig(pPlayerInfo.Hero));
		

		////todo: delete. test adding weapons
		//WeaponController.AddWeapon(
		//	brainiacs.ItemManager.GetPlayerWeaponConfig(EWeaponId.TestGun2));
		////todo: delete. test adding same weapon
		//WeaponController.AddWeapon(
		//	brainiacs.ItemManager.GetPlayerWeaponConfig(EWeaponId.TestGun2));


		//SPECIAL
		WeaponController.AddWeapon(
			brainiacs.ItemManager.GetHeroSpecialWeaponConfig(pPlayerInfo.Hero));

		////todo: delete. 
		//WeaponController.AddWeapon(
		//	brainiacs.ItemManager.GetPlayerWeaponConfig(EWeaponId.TestGun));

		//last one is active



		Visual.Init(spriteRend, brainiacs.HeroManager.GetHeroConfig(pPlayerInfo.Hero));

		Movement.SpawnAt(pSpawnPosition);

	}

	
}
