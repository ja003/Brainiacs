using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Generates items (powerUp, weapons, ..) on random locations evry X second.
/// Active only on master
/// </summary>
public class MapItemGenerator : GameBehaviour
{
	[SerializeField] MapItem mapItemPrefab = null;
	[SerializeField] private int frequency = -1;
	[SerializeField] private bool isActive = false;


	public void Init()
	{
		StartGenerating();

		if(!DebugData.StopGenerateItems)
		{
			//delay needed, otherwise can be generated on invalid location
			DoInTime(GenerateRandomItem, 0.5f); //DEBUG 
		}

	}

	private void StartGenerating()
	{
		if(DebugData.TestGenerateItems)
			frequency = 2;

		float time = Random.Range(frequency - 1, frequency + 1);
		DoInTime(OnGenerateCountdownFinished, time);
	}

	private void OnGenerateCountdownFinished()
	{
		if(isActive && !DebugData.StopGenerateItems)
		{
			GenerateRandomItem();
		}
		StartGenerating();
	}

	private void GenerateRandomItem()
	{
		EMapItem nextItemType = GetNextMapItemType();
		if(DebugData.TestGameEffect!= EGameEffect.None)
			nextItemType = EMapItem.GameEffect;
		else if(DebugData.TestPowerUp != EPowerUp.None)
			nextItemType = EMapItem.PowerUp;
		else if(DebugData.TestGenerateMapWeapon != EWeaponId.None)
			nextItemType = EMapItem.Weapon;
		else if(DebugData.TestGenerateMapSpecialWeapon != EWeaponId.None)
			nextItemType = EMapItem.SpecialWeapon;

		int randomIndex = GetRandomItemIndex(nextItemType);
		Vector2? randomGenPos = game.Map.ActiveMap.GetRandomPosition();
		if(randomGenPos == null)
		{
			Debug.Log("Skipping GenerateRandomItem");
			return;
		}
		MapItem newItem = InstanceFactory.Instantiate(mapItemPrefab.gameObject).GetComponent<MapItem>();

		Vector2 randomPosition = (Vector2)randomGenPos;
		switch(nextItemType)
		{
			case EMapItem.GameEffect:
				newItem.Init(randomPosition, MapItem.EType.GameEffect, randomIndex);

				break;

			case EMapItem.PowerUp:
			case EMapItem.PowerUp2:
			case EMapItem.PowerUp3:
				//if(DebugData.TestPowerUp != EPowerUp.None)
				//	randomIndex = (int)DebugData.TestPowerUp;

				newItem.Init(randomPosition, MapItem.EType.PowerUp, randomIndex);
				break;
			case EMapItem.Weapon:
			case EMapItem.Weapon2:
				newItem.Init(randomPosition, MapItem.EType.Weapon, randomIndex);
				break;
			case EMapItem.SpecialWeapon:
				newItem.Init(randomPosition, MapItem.EType.SpecialWeapon, randomIndex);
				break;
		}
	}

	private int GetRandomItemIndex(EMapItem pType)
	{
		int i;
		switch(pType)
		{
			case EMapItem.GameEffect:
				if(DebugData.TestGameEffect != EGameEffect.None)
					return (int)DebugData.TestGameEffect;

				i = Random.Range(0, brainiacs.ItemManager.GameEffects.Count);
				return (int)brainiacs.ItemManager.GameEffects[i].Type;

			case EMapItem.PowerUp:
			case EMapItem.PowerUp2:
			case EMapItem.PowerUp3:
				if(DebugData.TestPowerUp != EPowerUp.None)
					return (int)DebugData.TestPowerUp;

				i = Random.Range(0, brainiacs.ItemManager.PowerUps.Count);
				return (int)brainiacs.ItemManager.PowerUps[i].Type;

			case EMapItem.Weapon:
			case EMapItem.Weapon2:
				if(DebugData.TestGenerateMapWeapon != EWeaponId.None)
					return (int)DebugData.TestGenerateMapWeapon;

				i = Random.Range(0, brainiacs.ItemManager.MapWeapons.Count);
				return (int)brainiacs.ItemManager.MapWeapons[i].Id;
			
			case EMapItem.SpecialWeapon:
				if(DebugData.TestGenerateMapSpecialWeapon != EWeaponId.None)
					return (int)DebugData.TestGenerateMapSpecialWeapon;

				i = Random.Range(0, brainiacs.ItemManager.MapWeaponsSpecial.Count);
				return (int)brainiacs.ItemManager.MapWeaponsSpecial[i].Id;
		}
		Debug.LogError("Invalid item type");
		return -1;
	}

	EMapItem lastItemType;
	private EMapItem GetNextMapItemType()
	{
		int itemTypesCount = Enum.GetNames(typeof(EMapItem)).Length;
		int rand = Random.Range(0, itemTypesCount);
		EMapItem nextItem = (EMapItem)(((int)lastItemType + rand) % itemTypesCount);
		lastItemType = nextItem;
		return nextItem;
	}

	/// <summary>
	/// Map item types.
	/// Multiple type => higher chance to generate
	/// </summary>
	private enum EMapItem
	{
		PowerUp,
		PowerUp2,
		PowerUp3,
		Weapon,
		Weapon2,
		SpecialWeapon,
		GameEffect
	}

	///// <summary>
	///// Finds random position on map and returns it if there is no near object
	///// </summary>
	//private Vector2? GetRandomGeneratePosition(int pIteration = 0)
	//{

	//	Vector2 topLeft = game.Map.ActiveMap.TopLeftCorner.position;
	//	Vector2 botRight = game.Map.ActiveMap.BotRightCorner.position;

	//	Vector2 randomPos = new Vector2(Random.Range(topLeft.x, botRight.x), Random.Range(topLeft.y, botRight.y));
	//	Utils.DebugDrawCross(randomPos, Color.red, 1);

	//	if(CanGenerateOn(randomPos))
	//		return randomPos;

	//	const int max_iterations = 20;
	//	if(pIteration > max_iterations)
	//	{
	//		//not error - this means that there are probably too much items on map already
	//		// => no need to generate more
	//		Debug.Log("Couldnt find good position to generate item");
	//		return null;
	//	}

	//	return GetRandomGeneratePosition(pIteration + 1);
	//}

	/// <summary>
	/// raplaced by: GetRandomGeneratePosition
	/// </summary>
	//private Vector2 GetRandomPosition()
	//{
	//	if(DebugData.TestGenerateItems)
	//		return debug_GetRandomPosition();

	//	//return Vector2.up; //DEBUG
	//	Vector2 pos = game.Map.ActiveMap.GetRandomMapItemGenPos().position;

	//	//pos = Vector2.zero; //DEBUG

	//	Vector2 dirToCenter = (Vector2.zero - pos).normalized;
	//	//in case pos = ZERO
	//	if(dirToCenter.magnitude < 0.01f) dirToCenter = Vector2.right;
	//	int iter = 0;
	//	while(!CanGenerateOn(pos))
	//	{
	//		if(CanGenerateOn(pos + Vector2.up))
	//			return pos + Vector2.up;
	//		if(CanGenerateOn(pos + Vector2.right))
	//			return pos + Vector2.right;
	//		if(CanGenerateOn(pos + Vector2.down))
	//			return pos + Vector2.down;
	//		if(CanGenerateOn(pos + Vector2.left))
	//			return pos + Vector2.left;

	//		Utils.DebugDrawCross(pos, Color.red, 1);

	//		const float step = 0.5f;
	//		pos += dirToCenter * step;
	//		const int maxSteps = 10;
	//		if(iter++ > maxSteps)
	//		{
	//			Debug.Log("Couldnt find good position to generate item");
	//			break;
	//		}
	//	}

	//	return pos;

	//	//random pos approach
	//	//float x = Random.Range(topLeftCorner.x, botRightCorner.x);
	//	//float y = Random.Range(topLeftCorner.y, botRightCorner.y);
	//	//return new Vector2(x, y, 0);
	//}

	
}
