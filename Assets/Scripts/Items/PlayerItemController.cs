using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : PlayerBehaviour
{
	internal void Init(EHero pHero)
	{
		AddHeroBasicWeapon(pHero);
		AddHeroSpecialWeapon(pHero);

		weapon.SetDefaultWeaponActive();

		foreach(var weapon in player.InitInfo.debug_StartupWeapon)
		{
			AddWeapon(weapon);
		}
	}

	private void AddWeapon(EWeaponId pWeapon)
	{
		switch(brainiacs.ItemManager.GetWeaponCathegory(pWeapon))
		{
			case EWeaponCathegory.None:
				return;
			case EWeaponCathegory.HeroBasic:
				EHero hero = brainiacs.HeroManager.GetHeroOfWeapon(pWeapon);
				AddHeroBasicWeapon(hero);
				return;
			case EWeaponCathegory.HeroSpecial:
				hero = brainiacs.HeroManager.GetHeroOfWeapon(pWeapon);
				AddHeroSpecialWeapon(hero);
				return;
			case EWeaponCathegory.MapBasic:
				AddMapWeapon(pWeapon);
				return;
			case EWeaponCathegory.MapSpecial:
				AddMapWeaponSpecial(pWeapon);
				return;
		}
		Debug.LogError("Couldnt add weapon " + pWeapon);
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
