using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : PlayerBehaviour
{
	internal void AddHeroSpecialWeapon(EHero pHero)
	{		
		HeroSpecialWeaponConfig config =
			brainiacs.ItemManager.GetHeroSpecialWeaponConfig(pHero);

		if(config == null)
		{
			Debug.LogError($"Added weapon was null");
			return;
		}

		PlayerWeaponSpecial weaponSpecial = new PlayerWeaponSpecial(
			player, config);

		weapon.AddWeapon(weaponSpecial);
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
		PlayerWeaponProjectile weaponProjectile =
			new PlayerWeaponProjectile(player, config);
		weapon.AddWeapon(weaponProjectile);
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

		PlayerWeaponProjectile weaponProjectile =
			new PlayerWeaponProjectile(player, config);
		weapon.AddWeapon(weaponProjectile);
	}

	internal void AddMapWeaponSpecial(EWeaponId pWeapon)
	{
		if(pWeapon == EWeaponId.None)
		{
			Debug.LogError($"Added weapon was null");
			return;
		}
		MapSpecialWeaponConfig config =
			brainiacs.ItemManager.GetMapSpecialWeaponConfig(pWeapon);
		if(config == null)
			return;

		PlayerWeaponSpecial weaponSpecial =
			new PlayerWeaponSpecial(player, config);
		weapon.AddWeapon(weaponSpecial);
	}
}
