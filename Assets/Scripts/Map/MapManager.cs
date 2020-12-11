using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	[SerializeField] private List<MapConfig> mapConfigs = null;
	private Dictionary<EMap, MapConfig> mapConfigMap = new Dictionary<EMap, MapConfig>();

	bool isInited;

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		if(isInited)
			return;

		foreach(MapConfig config in mapConfigs)
		{
			mapConfigMap.Add(config.Id, config);
		}
		isInited = true;
	}

	public MapConfig GetMapConfig(EMap pMap)
	{
		//GetMapConfig is called on Awake so map configs might not be initialized yet
		Init();

		mapConfigMap.TryGetValue(pMap, out MapConfig config);
		if(config == null)
		{
			Debug.LogError($"No config for map {pMap} found");
			mapConfigMap.TryGetValue(EMap.Wonderland, out config);
		}

		return config;
	}
}
