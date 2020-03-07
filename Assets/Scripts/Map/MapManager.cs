using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	[SerializeField]
	private List<MapConfig> mapConfigs;
	private Dictionary<EMap, MapConfig> mapConfigMap = new Dictionary<EMap, MapConfig>();

	private void Awake()
	{
		foreach(MapConfig config in mapConfigs)
		{
			mapConfigMap.Add(config.Id, config);
		}
	}

	public MapConfig GetMapConfig(EMap pMap)
	{
		mapConfigMap.TryGetValue(pMap, out MapConfig config);
		if(config == null)
		{
			Debug.LogError($"No config for map {pMap} found");
			mapConfigMap.TryGetValue(EMap.Wonderland, out config);
		}

		return config;
	}
}
