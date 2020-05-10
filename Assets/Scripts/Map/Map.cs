using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : GameBehaviour
{
	[SerializeField] Transform spawnPointsHolder = null;
	[SerializeField] Transform mapItemGenPosHolder = null; 

	private List<Transform> spawnPoints = new List<Transform>();
	//positions where map items can be generated
	private List<Transform> mapItemGenPos = new List<Transform>();

	private List<int> assignedSpawnPoints;

	[SerializeField] public Transform TopLeftCorner;
	[SerializeField] public Transform BotRightCorner;

	internal void SetActive(bool pValue)
	{
		//register spawnpoints
		for(int i = 0; i < spawnPointsHolder.transform.childCount; i++)
		{
			spawnPoints.Add(spawnPointsHolder.GetChild(i));
		}
		if(spawnPoints.Count < 4)
		{
			Debug.LogError("Not enough spawnpoints defined");
		}

		//register positions for generating map items
		for(int i = 0; i < mapItemGenPosHolder.transform.childCount; i++)
		{
			mapItemGenPos.Add(mapItemGenPosHolder.GetChild(i));
		}
		if(mapItemGenPos.Count < 4)
		{
			Debug.LogError("Not enough map item gen pos defined");
		}

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

	int lastUsedPos;
	public Transform GetRandomMapItemGenPos()
	{
		int randIndex = UnityEngine.Random.Range(0, mapItemGenPos.Count);
		if(randIndex == lastUsedPos)
		{
			//Debug.Log("Select another pos");
			randIndex = randIndex++ % mapItemGenPos.Count;
		}
		lastUsedPos = randIndex;
		return mapItemGenPos[randIndex];
	}
}
