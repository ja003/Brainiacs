using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PowerupManager
{
	private const int SHIELD_DURATION = 5;
	//private const int SPEED_VALUE = 5;
	private const int SPEED_DURATION = 5;

	public static void HandlePowerup(PowerUpConfig pConfig, Player pPlayer)
	{
		//EPowerUp type = pConfig.Type;
		//Debug.Log($"HandlePowerup {type} for {pPlayer}");

		string statusMessage = ApplyPowerup(pConfig, pPlayer);
		//show status
		Game.Instance.PlayerStatusManager.ShowStatus(pPlayer.Stats.MapItemUiPosition.position,
			statusMessage.Length > 0 ? statusMessage : pConfig.MapItemInfo.StatusText,
			pConfig.MapItemInfo.StatusSprite);
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
				pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.DoubleSpeed, SPEED_DURATION);//, SPEED_VALUE);
				break;
			case EPowerUp.Mystery:
				return HandleMystery(pPlayer);
			case EPowerUp.Shield:
				pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.Shield, SHIELD_DURATION);
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
		const int chance_ammo = 5;
		const int chance_health = 10;
		const int chance_speed = 20;
		const int chance_shield = 30;
		const int chance_receive_damage = 40;

		//todo: general method for weighted random
		const int chance_effect_half_speed = 50;
		const int chance_effect_double_damage = 60;
		const int chance_effect_half_damage = 70;


		float random = Random.Range(0, chance_effect_half_damage);
		//Debug.Log($"random = {random}");
		if(DebugData.TestPlayerEffect != EPlayerEffect.None)
		{
			switch(DebugData.TestPlayerEffect)
			{
				case EPlayerEffect.None:
					break;
				case EPlayerEffect.DoubleSpeed:
					break;
				case EPlayerEffect.HalfSpeed:
					break;
				case EPlayerEffect.Shield:
					break;
				case EPlayerEffect.DoubleDamage:
					random = chance_effect_double_damage;
					break;
				case EPlayerEffect.HalfDamage:
					break;
				default:
					break;
			}
			random -= 1;
		}

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
		else if(random < chance_receive_damage)
		{
			pPlayer.Stats.AddHealth(-20);
			return "- health";
		}

		else if(random < chance_effect_half_speed)
		{
			pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.HalfSpeed, 5);
			return "- speed";
		}
		else if(random < chance_effect_double_damage)
		{
			pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.DoubleDamage, 5);
			return "+ damage";
		}
		else if(random < chance_effect_half_damage)
		{
			pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.HalfDamage, 5);
			return "- damage";
		}

		
		return ApplyPowerup(Brainiacs.Instance.ItemManager.GetPowerupConfig(type), pPlayer);
	}
}
