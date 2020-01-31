using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : GameBehaviour
{
	[SerializeField]
	public List<PlayerWeaponConfig> playerWeapons;

	[SerializeField]
	private List<PowerUpConfig> powerUps;

	Dictionary<EWeaponId, PlayerWeaponConfig> playerWeaponMap = 
		new Dictionary<EWeaponId, PlayerWeaponConfig>();

	public List<PlayerItemConfig> MapItems = new List<PlayerItemConfig>();

	protected override void Awake()
	{
		base.Awake();

		foreach(PlayerWeaponConfig config in playerWeapons)
		{
			playerWeaponMap.Add(config.Id, config);
			if(config.CanDropOnMap)
			{
				MapItems.Add(config);
			}
		}
		foreach(PowerUpConfig config in powerUps)
		{
			//do we need to store powerups?
			//playerWeaponMap.Add(config.Id, config);
			if(config.CanDropOnMap)
			{
				MapItems.Add(config);
			}
		}
	}

	public PlayerWeaponConfig GetPlayerWeaponConfig(EWeaponId pId)
	{
		PlayerWeaponConfig item;
		playerWeaponMap.TryGetValue(pId, out item);
		return item;
	}

	public PlayerWeaponConfig GetHeroWeaponConfig(EHero pHero)
	{
		return GetPlayerWeaponConfig(EWeaponId.TestGun);
	}
}
