using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : BrainiacsController
{
	[SerializeField]
	public List<HeroConfig> heroConfigs;
	private Dictionary<EHero, HeroConfig> heroConfigMap = new Dictionary<EHero, HeroConfig>();

	protected override void OnMainControllerAwaken()
	{
		foreach(HeroConfig config in heroConfigs)
		{
			heroConfigMap.Add(config.Hero, config);
		}
	}

	public HeroConfig GetHeroConfig(EHero pHero)
	{
		heroConfigMap.TryGetValue(pHero, out HeroConfig config);
		if(config == null)
		{
			Debug.LogError($"No config for hero {pHero} found");
			heroConfigMap.TryGetValue(EHero.Tesla, out config);
		}

		return config;
	}

	internal EHero GetHeroOfWeapon(EWeaponId pWeapon)
	{
		switch(pWeapon)
		{
			case EWeaponId.Special_Curie:
				return EHero.Currie;
			case EWeaponId.Special_DaVinci:
				return EHero.DaVinci;
			case EWeaponId.Special_Einstein:
				return EHero.Einstein;
			case EWeaponId.Special_Nobel:
				return EHero.Nobel;
			case EWeaponId.Special_Tesla:
				return EHero.Tesla;
			case EWeaponId.Basic_Curie:
				return EHero.Currie;
			case EWeaponId.Basic_DaVinci:
				return EHero.DaVinci;
			case EWeaponId.Basic_Einstein:
				return EHero.Einstein;
			case EWeaponId.Basic_Nobel:
				return EHero.Nobel;
			case EWeaponId.Basic_Tesla:
				return EHero.Tesla;
		}

		Debug.LogError(pWeapon + " has no hero assigned");
		return EHero.None;
	}

	//not neccessary?
	//public List<EHero> GetAllHeroes()
	//{

	//	foreach(EHero dir in Enum.GetValues(typeof(EHero)))
	//	{
	//		if(movementRequested = IsMovementRequested(dir))
	//		{
	//			movement.Move(dir);
	//			break;
	//		}
	//	}
	//}
}
