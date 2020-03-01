using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemManager : GameBehaviour
{
	[SerializeField]
	public List<MapWeaponConfig> MapWeapons;

	[SerializeField]
	public List<MapSpecialWeapon> MapWeaponsSpecial;


	[SerializeField]
	public List<PowerUpConfig> PowerUps;

	public MapSpecialWeapon GetMapSpecialWeaponConfig(EWeaponId pId)
	{
		return MapWeaponsSpecial.Find(a => a.Id == pId);
	}

	public MapWeaponConfig GetMapWeaponConfig(EWeaponId pId)
	{

		MapWeaponConfig config = MapWeapons.Find(a => a.Id == pId);
		if(config == null)
			Debug.LogError($"Config for weapon {pId} not found");

		return config;
	}

	public HeroSpecialWeaponConfig GetHeroSpecialWeaponConfig(EHero pHero)
	{
		HeroConfig heroConfig = brainiacs.HeroManager.GetHeroConfig(pHero);
		return heroConfig.SpecialWeapon;
	}

	public HeroBasicWeaponConfig GetHeroBasicWeaponConfig(EHero pHero)
	{
		HeroConfig heroConfig = brainiacs.HeroManager.GetHeroConfig(pHero);
		return heroConfig.BasicWeapon;
	}
}
