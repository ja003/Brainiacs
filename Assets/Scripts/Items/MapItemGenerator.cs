using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItemGenerator : GameController
{
	Vector3 topLeftCorner;
	Vector3 botRightCorner;

	[SerializeField]
	MapItem mapItemPrefab;
	
	[SerializeField]
	private int frequency;

	[SerializeField]
	private bool isActive;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnGameAwaken()
	{
	}

	protected override void OnGameActivated()
	{
		topLeftCorner = game.MapController.ActiveMap.TopLeftCorner.position;
		botRightCorner = game.MapController.ActiveMap.BotRightCorner.position;

		StartGenerating();

		GenerateRandomItem(); //DEBUG
	}

	private void StartGenerating()
	{
		float time = Random.Range(frequency -1, frequency + 1);
		DoInTime(OnGenerateCountdownFinished, time);
	}

	private void OnGenerateCountdownFinished()
	{
		if(isActive)
		{
			GenerateRandomItem();
		}
		StartGenerating();
	}

	private void GenerateRandomItem()
	{
		MapItem newItem = Instantiate(mapItemPrefab, GetHolder());
		int randomIndex = Random.Range(0, brainiacs.ItemManager.MapItems.Count);

		randomIndex = 0; //debug
		MapItemConfig itemConfig =
			brainiacs.ItemManager.MapItems[randomIndex];
		newItem.Spawn(GetRandomPosition(), itemConfig);
	}

	private Vector3 GetRandomPosition()
	{
		return Vector3.up; //DEBUG

		float x = Random.Range(topLeftCorner.x, botRightCorner.x);
		float y = Random.Range(topLeftCorner.y, botRightCorner.y);
		return new Vector3(x, y, 0);
	}
}
