using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	[SerializeField]
	private List<Transform> spawnPoints;

	private List<int> assignedSpawnPoints;

	[SerializeField]
	public Transform TopLeftCorner;
	[SerializeField]
	public Transform BotRightCorner;

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
			int index = i % spawnPoints.Count;
			if(assignedSpawnPoints.Contains(index))
				continue;

			assignedSpawnPoints.Add(index);
			return spawnPoints[index];
		}
		return null;
	}

	public Transform GetSpawnPoint(int pIndex)
	{
		return spawnPoints[pIndex];
	}
}
