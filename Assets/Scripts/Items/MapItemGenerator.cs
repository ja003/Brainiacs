using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates items (powerUp, weapons, ..) on random locations evry X second.
/// Active only on master
/// </summary>
public class MapItemGenerator : GameBehaviour
{
	Vector3 topLeftCorner;
	Vector3 botRightCorner;

	[SerializeField] MapItem mapItemPrefab = null;
	[SerializeField] private int frequency = -1;
	[SerializeField] private bool isActive = false;


	public void Init()
	{
		topLeftCorner = game.Map.ActiveMap.TopLeftCorner.position;
		botRightCorner = game.Map.ActiveMap.BotRightCorner.position;

		StartGenerating();

		GenerateRandomItem(); //DEBUG
	}

	private void StartGenerating()
	{
		float time = Random.Range(frequency - 1, frequency + 1);
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
		MapItem newItem = InstanceFactory.Instantiate(mapItemPrefab.gameObject).GetComponent<MapItem>();
		int randomIndex = Random.Range(0, brainiacs.ItemManager.MapWeapons.Count);

		//todo: pick from powerup, mapWeapon, mapSpecialWeapon
		//or try tu unify them

		//NewMapWeaponConfig newMapWeaponConfig = brainiacs.ItemManager.MapWeapons[randomIndex];
		//newItem.Spawn(GetRandomPosition(), newMapWeaponConfig);

		randomIndex = Random.Range(0, brainiacs.ItemManager.PowerUps.Count);
		PowerUpConfig powerUpConfig = brainiacs.ItemManager.PowerUps[randomIndex];
		newItem.Spawn(GetRandomPosition(), powerUpConfig);


		//TODO: other map items!!!
	}

	private Vector3 GetRandomPosition()
	{
		//return Vector3.up; //DEBUG

		float x = Random.Range(topLeftCorner.x, botRightCorner.x);
		float y = Random.Range(topLeftCorner.y, botRightCorner.y);
		return new Vector3(x, y, 0);
	}
}
