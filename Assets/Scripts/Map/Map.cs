﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Map : GameBehaviour, IPositionValidator
{
	[SerializeField] Transform spawnPointsHolder = null;
	[SerializeField] Transform mapItemGenPosHolder = null;


	private List<Transform> spawnPoints = new List<Transform>();
	//positions where map items can be generated
	private List<Transform> mapItemGenPos = new List<Transform>();

	private List<int> assignedSpawnPoints;

	[SerializeField] public Transform TopLeftCorner;
	[SerializeField] public Transform BotRightCorner;


	[SerializeField] bool debug_DrawGrid;
	[SerializeField] Transform debug_ItemGenPos = null;
	[SerializeField] Transform debug_Respawnpoint = null;

	protected override void Awake()
	{
		if(debug_ItemGenPos != null)
			debug_RandomPosition = Utils.GetVector2(debug_ItemGenPos.position);
	}

	private void Update()
	{
		if(debug_DrawGrid)
			AstarAdapter.Debug_DrawGrid();
	}

	internal void SetActive(bool pValue)
	{
		if(pValue)
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

	//int lastUsedPos;
	//Replaced by GetRandomPosition
	//public Transform GetRandomMapItemGenPos()
	//{
	//	int randIndex = UnityEngine.Random.Range(0, mapItemGenPos.Count);
	//	if(randIndex == lastUsedPos)
	//	{
	//		//Debug.Log("Select another pos");
	//		randIndex = randIndex++ % mapItemGenPos.Count;
	//	}
	//	lastUsedPos = randIndex;
	//	return mapItemGenPos[randIndex];
	//}

	public Vector2 GetRespawnPosition()
	{
		if(debug.Respawnpoint)
			return debug_Respawnpoint.position;

		Vector2? respawnPos = GetRandomPosition();
		if(respawnPos == null)
		{
			Debug.Log("Couldnt find good spawn position => spawn at center");
			respawnPos = Vector2.zero;
		}
		return (Vector2)respawnPos;
	}

	/// <summary>
	/// Finds random position on map and returns it if the pCondition is met
	/// </summary>
	public Vector2? GetRandomPosition(IPositionValidator pCondition = null, int pIteration = 0)
	{
		if(pCondition == null)
			pCondition = this;

		if(debug.GenerateItems)
			return debug_GetRandomPosition();

		Vector2 topLeft = TopLeftCorner.position;
		Vector2 botRight = BotRightCorner.position;

		Vector2 randomPos = new Vector2(Random.Range(topLeft.x, botRight.x), Random.Range(topLeft.y, botRight.y));
		Utils.DebugDrawCross(randomPos, Color.red, 1);

		if(pCondition.IsPositionValid(randomPos, 1))
			return randomPos;

		const int max_iterations = 20;
		if(pIteration > max_iterations)
		{
			//not error - this means that there are probably too much items on map already
			// => no need to generate more
			Debug.Log("Couldnt find good position to generate item");
			return null;
		}

		return GetRandomPosition(pCondition, pIteration + 1);
	}

	/// <summary>
	/// Default position validator.
	/// Position is valid if no object if too close.
	/// </summary>
	public bool IsPositionValid(Vector2 pPosition, float pMinDistance)
	{
		//not too close to player
		foreach(var player in game.PlayerManager.Players)
		{
			if(player.GetDistance(pPosition) < 2 * PlayerVisual.PlayerBodySize)
				return false;
		}

		//cant overlap with anything (player, projectile, map object or another map item)
		if(Physics2D.OverlapBox(pPosition, Vector2.one * pMinDistance, 0))
			return false;

		return true;
	}

	public bool IsWithinMap(Vector2 pPoint)
	{
		return pPoint.x >= TopLeftCorner.position.x
			&& pPoint.x <= BotRightCorner.position.x
			&& pPoint.y >= BotRightCorner.position.y
			&& pPoint.y <= TopLeftCorner.position.y;
	}

	public Vector2 GetPositionWithinMap(Vector2 pPosition)
	{
		Vector2 clamped = new Vector2(
			Mathf.Clamp(pPosition.x, TopLeftCorner.position.x, BotRightCorner.position.x),
			Mathf.Clamp(pPosition.y, BotRightCorner.position.y, TopLeftCorner.position.y));
		return clamped;
	}

	Vector2 debug_RandomPosition = Vector2.left * 2;
	private Vector2 debug_GetRandomPosition(int pIterCount = 0)
	{
		const float item_pos_step = 0.5f;

		if(!IsWithinMap(debug_RandomPosition))
			debug_RandomPosition = debug_ItemGenPos.position;

		if(pIterCount > 100)
		{
			Debug.LogError("too many iterations");
			return debug_ItemGenPos.position;
		}

		//generate items in straight line
		if(!IsPositionValid(debug_RandomPosition, item_pos_step - 0.1f))
		{
			debug_RandomPosition += Vector2.right * item_pos_step;

			if(debug_RandomPosition.x > debug_ItemGenPos.position.x + 3)
			{
				debug_RandomPosition.x = debug_ItemGenPos.position.x;
				debug_RandomPosition.y += item_pos_step;
			}



			//Debug.Log("another pos");
			return debug_GetRandomPosition(pIterCount++);
		}



		return debug_RandomPosition;

	}
}

public interface IPositionValidator
{
	bool IsPositionValid(Vector2 pPosition, float pMinDistance);
}