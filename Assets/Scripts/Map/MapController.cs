using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : GameController
{
	public Map ActiveMap = null;

	protected override void Awake()
	{
		base.Awake();
	}

	public void SetMap(EMap pMap)
	{
		if(transform.childCount > 0)
		{
			ActiveMap = transform.GetChild(0).GetComponent<Map>();
			if(ActiveMap != null)
			{
				Debug.LogWarning("Map is already present in the scene");
				if(!Brainiacs.SelfInitGame)
					Debug.LogError("Map object cant be present in the scene [not error if debugging]");
				return;
			}
		}

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
		//gameObject.SetActive(pValue);
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
		SetActive(true);
	}
}
