using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
	[SerializeField]
	private List<HeroConfig> heroConfigs;
	private Dictionary<EHero, HeroConfig> heroConfigMap = new Dictionary<EHero, HeroConfig>();

	private void Awake()
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

}
