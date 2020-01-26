using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
	[SerializeField]
	private Map steampunk;

	[SerializeField]
	private Map wonderland;

	public Map ActiveMap = null;

	public void SetMap(EMap pMap)
	{
		if(transform.childCount > 0)
		{
			ActiveMap = transform.GetChild(0).GetComponent<Map>();
			if(ActiveMap != null)
			{
				Debug.LogWarning("Map is already present in the scene");
				if(!Brainiacs.SelfInitGame)
					Debug.LogError("Map object cant be present in the scene");
				return;
			}
		}

		switch(pMap)
		{
			case EMap.Steampunk:
				ActiveMap = Instantiate(steampunk);
				break;
			case EMap.Wonderland:
				ActiveMap = Instantiate(wonderland);
				break;
		}
		if(ActiveMap == null)
		{
			Debug.LogError("No map selected");
			return;
		}

		ActiveMap.transform.parent = transform;
	}

	internal void SetActive(bool pValue)
	{
		gameObject.SetActive(pValue);
		ActiveMap.SetActive(pValue);
	}
}
