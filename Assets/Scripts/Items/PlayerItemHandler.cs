using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHandler
{
	private PlayerStats stats;

	private PlayerWeaponController weaponController;

	public PlayerItemHandler(PlayerStats playerStats, PlayerWeaponController playerWeaponController)
	{
		stats = playerStats;
		weaponController = playerWeaponController;
	}

	public void OnEnterPowerUp(PowerUpConfig pPowerUpConfig)
	{
		Debug.Log($"OnEnterPowerUp {pPowerUpConfig}");
		switch(pPowerUpConfig.Id)
		{
			case EPowerUp.Random:
				return;
			case EPowerUp.Health:
				stats.Health += (int)pPowerUpConfig.Value;
				return;
			case EPowerUp.Speed:
				stats.SetSpeed(pPowerUpConfig.Value, 5);
				return;
		}
		Debug.LogError("Unknown power up");
	}

	public void OnEnterWeapon(PlayerWeaponConfig pPlayerWeaponConfig)
	{
		weaponController.AddWeapon(pPlayerWeaponConfig);
	}
}
