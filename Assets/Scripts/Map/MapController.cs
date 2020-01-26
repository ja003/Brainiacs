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

	private Map activeMap = null;

	public void SetMap(EMap pMap)
	{
		if(transform.childCount > 0)
		{
			activeMap = transform.GetChild(0).GetComponent<Map>();
			if(activeMap != null)
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
				activeMap = Instantiate(steampunk);
				break;
			case EMap.Wonderland:
				activeMap = Instantiate(wonderland);
				break;
		}
		if(activeMap == null)
		{
			Debug.LogError("No map selected");
			return;
		}

		activeMap.transform.parent = transform;
	}

	internal void SetActive(bool pValue)
	{
		gameObject.SetActive(pValue);
		activeMap.SetActive(pValue);
	}
}
