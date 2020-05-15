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

		if(!DebugData.StopGenerateItems)
			GenerateRandomItem(); //DEBUG 
	}

	private void StartGenerating()
	{
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
		MapItem newItem = InstanceFactory.Instantiate(mapItemPrefab.gameObject).GetComponent<MapItem>();
		//MapItem newItem = game.Pool.Instantiate(mapItemPrefab.gameObject).GetComponent<MapItem>();

		EMapItem nextItemType = GetNextMapItemType();
		if(DebugData.TestPowerUp != EPowerUp.None)
			nextItemType = EMapItem.PowerUp;
		else if(DebugData.TestMapWeapon != EWeaponId.None)
			nextItemType = EMapItem.Weapon;
		else if(DebugData.TestMapSpecialWeapon != EWeaponId.None)
			nextItemType = EMapItem.SpecialWeapon;

		int randomIndex = GetRandomItemIndex(nextItemType);
		Vector3 randomPosition = GetRandomPosition();
		switch(nextItemType)
		{
			case EMapItem.PowerUp:
			case EMapItem.PowerUp2:
			case EMapItem.PowerUp3:
				PowerUpConfig powerUpConfig = brainiacs.ItemManager.PowerUps[randomIndex];
				if(DebugData.TestPowerUp != EPowerUp.None)
					powerUpConfig = brainiacs.ItemManager.PowerUps.Find(a => a.Type == DebugData.TestPowerUp);

				newItem.Init(randomPosition, powerUpConfig);
				break;
			case EMapItem.Weapon:
			case EMapItem.Weapon2:
				MapWeaponConfig weapon = brainiacs.ItemManager.MapWeapons[randomIndex];
				newItem.Init(randomPosition, weapon);
				break;
			case EMapItem.SpecialWeapon:
				MapSpecialWeaponConfig specialWeapon = brainiacs.ItemManager.MapWeaponsSpecial[randomIndex];
				newItem.Init(randomPosition, specialWeapon);
				break;
		}
	}

	private int GetRandomItemIndex(EMapItem pType)
	{
		switch(pType)
		{
			case EMapItem.PowerUp:
			case EMapItem.PowerUp2:
			case EMapItem.PowerUp3:
				return Random.Range(0, brainiacs.ItemManager.PowerUps.Count);
			case EMapItem.Weapon:
			case EMapItem.Weapon2:
				if(DebugData.TestMapWeapon != EWeaponId.None)
				{
					return brainiacs.ItemManager.MapWeapons
						  .FindIndex(a => a.Id == DebugData.TestMapWeapon);
				}
				return Random.Range(0, brainiacs.ItemManager.MapWeapons.Count);
			case EMapItem.SpecialWeapon:
				if(DebugData.TestMapSpecialWeapon != EWeaponId.None)
				{
					return brainiacs.ItemManager.MapWeaponsSpecial
						  .FindIndex(a => a.Id == DebugData.TestMapSpecialWeapon);
				}
				return Random.Range(0, brainiacs.ItemManager.MapWeaponsSpecial.Count);
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
		SpecialWeapon
	}

	private Vector3 GetRandomPosition()
	{
		if(DebugData.TestGenerateItems)
			return debug_GetRandomPosition();

		//return Vector3.up; //DEBUG
		Vector3 pos = game.Map.ActiveMap.GetRandomMapItemGenPos().position;

		//pos = Vector3.zero; //DEBUG

		Vector3 dirToCenter = (Vector3.zero - pos).normalized;
		//in case pos = ZERO
		if(dirToCenter.magnitude < 0.01f) dirToCenter = Vector3.right;
		int iter = 0;
		while(!CanGenerateOn(pos))
		{
			if(CanGenerateOn(pos + Vector3.up))
				return pos + Vector3.up;
			if(CanGenerateOn(pos + Vector3.right))
				return pos + Vector3.right;
			if(CanGenerateOn(pos + Vector3.down))
				return pos + Vector3.down;
			if(CanGenerateOn(pos + Vector3.left))
				return pos + Vector3.left;

			Utils.DebugDrawCross(pos, Color.red, 1);

			const float step = 0.5f;
			pos += dirToCenter * step;
			const int maxSteps = 10;
			if(iter++ > maxSteps)
			{
				Debug.Log("Couldnt find good position to generate item");
				break;
			}
		}

		return pos;

		//random pos approach
		//float x = Random.Range(topLeftCorner.x, botRightCorner.x);
		//float y = Random.Range(topLeftCorner.y, botRightCorner.y);
		//return new Vector3(x, y, 0);
	}

	Vector3 debug_RandomPosition = Vector3.left * 3;
	private Vector3 debug_GetRandomPosition()
	{
		//generate items in straight line
		if(!CanGenerateOn(debug_RandomPosition))
		{
			debug_RandomPosition += Vector3.right * 0.5f;
			//Debug.Log("another pos");
		}

		if(debug_RandomPosition.x > 5)
			debug_RandomPosition = Vector3.left * 3;

		return debug_RandomPosition;

	}

	private bool CanGenerateOn(Vector3 pPosition)
	{
		//cant generate too close to player
		foreach(var player in game.PlayerManager.Players)
		{
			if(player.GetDistance(pPosition) < 2 * PlayerVisual.PlayerBodySize)
				return false;
		}

		//cant overlap with anything (player, projectile, map object or another map item)
		if(Physics2D.OverlapBox(pPosition, Vector2.one, 0))
			return false;

		return true;
	}
}
