using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : GameController
{
	Vector3 topLeftCorner;
	Vector3 botRightCorner;

	[SerializeField]
	MapItem mapItemPrefab;

	[SerializeField]
	Transform mapItemsHolder;

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
	}

	private void StartGenerating()
	{
		float time = Random.Range(1, 2);
		DoInTime(GenerateRandomItem, time);
	}

	public void GenerateRandomItem()
	{
		MapItem newItem = Instantiate(mapItemPrefab, mapItemsHolder);
		int randomIndex = Random.Range(0, brainiacs.ItemManager.MapItems.Count);
		newItem.Spawn(GetRandomPosition(), brainiacs.ItemManager.MapItems[randomIndex]);
		StartGenerating();
	}

	private Vector3 GetRandomPosition()
	{
		float x = Random.Range(topLeftCorner.x, botRightCorner.x);
		float y = Random.Range(topLeftCorner.y, botRightCorner.y);
		return new Vector3(x, y, 0);
	}
}
