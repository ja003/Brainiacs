﻿using System.Collections;
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
