using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : GameController
{
	[SerializeField]
	private PlayerStats stats;

	[SerializeField]
	private PlayerWeaponController weaponController;

	[SerializeField]
	private Player player;


	protected override void OnGameActivated()
	{
	}

	protected override void OnGameAwaken()
	{
	}

	internal void AddHeroSpecialWeapon(EHero pHero)
	{		
		HeroSpecialWeaponConfig config =
			brainiacs.ItemManager.GetHeroSpecialWeaponConfig(pHero);

		if(config == null)
		{
			Debug.LogError($"Added weapon was null");
			return;
		}

		PlayerWeaponSpecial weapon = new PlayerWeaponSpecial(
			player, config);

		weaponController.AddWeapon(weapon);
	}

	internal void AddHeroBasicWeapon(EHero pHero)
	{
		HeroBasicWeaponConfig config =
			brainiacs.ItemManager.GetHeroBasicWeaponConfig(pHero);
		if(config == null)
		{
			Debug.LogError($"Added weapon was null");
			return;
		}
		PlayerWeaponProjectile weapon =
			new PlayerWeaponProjectile(player, config);
		weaponController.AddWeapon(weapon);
	}

	//TODO: create system to check weapon cathegory
	internal void AddMapWeapon(EWeaponId pWeapon)
	{
		if(pWeapon == EWeaponId.None)
		{
			Debug.LogError($"Added weapon was null");
			return;
		}
		MapWeaponConfig config =
			brainiacs.ItemManager.GetMapWeaponConfig(pWeapon);
		if(config == null)
			return;

		PlayerWeaponProjectile weapon =
			new PlayerWeaponProjectile(player, config);
		weaponController.AddWeapon(weapon);
	}
}
