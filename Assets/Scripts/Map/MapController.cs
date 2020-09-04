using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : GameController
{
	public Map ActiveMap = null;

	[SerializeField] public MapItemManager Items;
	[SerializeField] public MapPhoton Photon;

	//all obstackles on map defined by their name-hash
	Dictionary<int, MapObstackle> obstackles = new Dictionary<int, MapObstackle>();

	protected override void Awake()
	{
		base.Awake();
	}


	public void SetMap(EMap pMap)
	{
		SoundController.PlayMusic(ESound.Music_Game);

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

		ActiveMap.transform.parent = transform;
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
	}

	protected override void OnMainControllerActivated()
	{
		Items.Init();
		SetActive(true);
	}

	/// OBSTACKLES

	internal void RegisterObstackle(MapObstackle pObstackle)
	{
		int id = pObstackle.gameObject.name.GetHashCode();// Utils.GenerateHash(pObstackle.gameObject.name);
		if(obstackles.ContainsKey(id))
		{
			Debug.LogError($"Obstackle {pObstackle.gameObject.name} already registered. Name has to be unique");
			return;
		}
		obstackles.Add(id, pObstackle);
	}


	internal void UnregisterObstackle(MapObstackle pObstackle)
	{
		obstackles.Remove(pObstackle.gameObject.name.GetHashCode());
	}

	internal MapObstackle GetObstackle(int pId)
	{
		MapObstackle result;
		obstackles.TryGetValue(pId, out result);
		if(!result)
			Debug.LogError($"Obstackle {pId} not found");
		return result;
	}
}
