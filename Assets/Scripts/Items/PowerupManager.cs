using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PowerupManager
{
	private const int SHIELD_DURATION = 5;
	private const int SPEED_VALUE = 5;
	private const int SPEED_DURATION = 5;

	public static void HandlePowerup(PowerUpConfig pConfig, Player pPlayer)
	{
		//EPowerUp type = pConfig.Type;
		//Debug.Log($"HandlePowerup {type} for {pPlayer}");

		string statusMessage = ApplyPowerup(pConfig, pPlayer);
		//show status
		Game.Instance.PlayerStatusManager.ShowStatus(pPlayer.Stats.MapItemUiPosition.position,
			statusMessage.Length > 0 ? statusMessage : pConfig.MapItemInfo.StatusText,
			pConfig.MapItemInfo.MapSprite);
	}

	/// <summary>
	/// Applies powerup to the player and returns status text
	/// </summary>
	private static string ApplyPowerup(PowerUpConfig pConfig, Player pPlayer)
	{
		switch(pConfig.Type)
		{
			case EPowerUp.Health:
				pPlayer.Stats.AddHealth(20);
				break;
			case EPowerUp.Ammo:
				pPlayer.WeaponController.OnPowerUpAmmo();
				break;
			case EPowerUp.Speed:
				pPlayer.Stats.SetSpeed(SPEED_VALUE, SPEED_DURATION);
				break;
			case EPowerUp.Mystery:
				return HandleMystery(pPlayer);
			case EPowerUp.Shield:
				pPlayer.Stats.SetShield(SHIELD_DURATION);
				break;
			default:
				Debug.LogError($"Powerup {pConfig.Type} not handled!");
				break;
		}
		return pConfig.MapItemInfo.StatusText;
	}

	/// <summary>
	/// Randomly applies some type of powerup
	/// </summary>
	private static string HandleMystery(Player pPlayer)
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
			return "- health";
		}
		else if(random < chance_double_damage)
		{
			pPlayer.Stats.AddHealth(-40);
			return "- health";
		}
		
		return ApplyPowerup(Brainiacs.Instance.ItemManager.GetPowerupConfig(type), pPlayer);
	}
}
