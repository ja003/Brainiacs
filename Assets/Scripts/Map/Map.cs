using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	[SerializeField]
	private List<Transform> spawnPoints;

	private List<int> assignedSpawnPoints;

	internal void SetActive(bool pValue)
	{
		gameObject.SetActive(pValue);
		assignedSpawnPoints = new List<int>();
	}

	public Transform GetSpawnPoint()
	{
		int randIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
		for(int i = randIndex; i < randIndex + spawnPoints.Count; i++)
		{
			if(assignedSpawnPoints.Contains(i))
				continue;

			assignedSpawnPoints.Add(i);
			return spawnPoints[i];
		}
		return null;
	}
}
