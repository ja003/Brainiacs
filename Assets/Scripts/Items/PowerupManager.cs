using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PowerupManager
{
	public static void HandlePowerup(PowerUpConfig pConfig, Player pPlayer)
	{
		EPowerUp type = pConfig.Type;
		//Debug.Log($"HandlePowerup {type} for {pPlayer}");

		ApplyPowerup(pConfig.Type, pPlayer);
		ShowStatus(pPlayer.Stats.StatusUiPosition.position, pConfig);		
	}

	private static void ApplyPowerup(EPowerUp pType, Player pPlayer)
	{
		switch(pType)
		{
			case EPowerUp.Health:
				pPlayer.Stats.AddHealth(20);
				return;
			case EPowerUp.Ammo:
				pPlayer.WeaponController.OnPowerUpAmmo();
				return;
			case EPowerUp.Speed:
				pPlayer.Stats.SetSpeed(5, 5);
				return;
			case EPowerUp.Mystery:
				HandleMystery(pPlayer);
				return;
			case EPowerUp.Shield:
				pPlayer.Stats.SetShield(5);
				return;
		}
		Debug.LogError($"Powerup {pType} not handled!");
	}

	private static void ShowStatus(Vector3 position, PowerUpConfig pConfig)
	{
		Game.Instance.PlayerStatusManager.SpawnAt(
			position,
			pConfig.MapItemInfo.StatusSprite,
			pConfig.MapItemInfo.StatusText);
	}

	private static void HandleMystery(Player pPlayer)
	{
		const int chance_ammo = 20;
		const int chance_health = 40;
		const int chance_speed = 60;
		const int chance_shield = 70;
		const int chance_damage = 80;
		const int chance_double_damage = 100;
		float random = Random.Range(0, chance_double_damage);
		//Debug.Log($"random = {random}");

		EPowerUp type = EPowerUp.None;

		if(random < chance_ammo)
		{
			type = EPowerUp.Ammo;
		}
		else if(random < chance_health)
		{
			type = EPowerUp.Health;
		}
		else if(random < chance_speed)
		{
			type = EPowerUp.Speed;
		}
		else if(random < chance_shield)
		{
			type = EPowerUp.Shield;
		}
		else if(random < chance_damage)
		{
			pPlayer.Stats.AddHealth(-20);
			return;
		}
		else if(random < chance_double_damage)
		{
			pPlayer.Stats.AddHealth(-40);
			return;
		}

		ApplyPowerup(type, pPlayer);
	}
}
