using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : GameController
{
	public Map ActiveMap = null;

	[SerializeField] public MapItemManager Items;
	[SerializeField] public MapPhoton Photon;

	//all obstacles on map defined by their name-hash
	Dictionary<int, MapObstacle> obstacles = new Dictionary<int, MapObstacle>();

	[SerializeField] public PathFinderController PathFinder;

	protected override void Awake()
	{
		base.Awake();
	}


	public void SetMap(EMap pMap)
	{
		brainiacs.AudioManager.PlayMusic(ESound.Music_Game);

		for(int i = 0; i < transform.childCount; i++)
		{
			ActiveMap = transform.GetChild(i).GetComponent<Map>();
			ActiveMap.SetActive(false);
			if(ActiveMap != null && ActiveMap.name.Contains(pMap.ToString()))
			{
				Debug.LogWarning("Map is already present in the scene");
				if(!Brainiacs.SelfInitGame)
					Debug.LogError("Map object cant be present in the scene [not error if debugging]");

				ActiveMap.SetActive(true);
				return;
			}
		}

		//if(transform.childCount > 0)
		//{
		//	ActiveMap = transform.GetChild(0).GetComponent<Map>();
		//	if(ActiveMap != null)
		//	{
		//		Debug.LogWarning("Map is already present in the scene");
		//		if(!Brainiacs.SelfInitGame)
		//			Debug.LogError("Map object cant be present in the scene [not error if debugging]");
		//		return;
		//	}
		//}

		ActiveMap = Instantiate(brainiacs.MapManager.GetMapConfig(pMap).Prefab);
		if(ActiveMap == null)
		{
			Debug.LogError("No map selected");
			return;
		}

		ActiveMap.transform.SetParent(transform);

	}



	public new void SetActive(bool pValue)
	{
		base.SetActive(pValue);
		//Debug.Log($"{gameObject.name} SetActive {pValue }");
		ActiveMap.SetActive(pValue);

		if(pValue)
			Activate();
	}

	protected override void OnMainControllerAwaken()
	{
		SetMap(brainiacs.GameInitInfo.Map);
		SetActive(false);
		//init pathfinder after SetMap
		PathFinder.Init(AiMovement.PATH_STEP, ActiveMap.TopLeftCorner.position, ActiveMap.BotRightCorner.position);
	}

	protected override void OnMainControllerActivated()
	{
		Items.Init();
		SetActive(true);
	}

	/// OBSTACLES

	internal void RegisterObstacle(MapObstacle pObstacle)
	{
		int id = pObstacle.gameObject.name.GetHashCode();// Utils.GenerateHash(pObstacle.gameObject.name);
		if(obstacles.ContainsKey(id))
		{
			Debug.LogError($"Obstacle {pObstacle.gameObject.name} already registered. Name has to be unique");
			return;
		}
		//Debug.Log($"RegisterObstacle {pObstacle.gameObject.name} = {id}");
		obstacles.Add(id, pObstacle);
	}


	internal void UnregisterObstacle(MapObstacle pObstacle)
	{
		obstacles.Remove(pObstacle.gameObject.name.GetHashCode());
	}

	internal MapObstacle GetObstacle(int pId)
	{
		MapObstacle result;
		obstacles.TryGetValue(pId, out result);
		if(!result)
			Debug.LogError($"Obstacle {pId} not found");
		return result;
	}
}
