using UnityEngine;
using Random = UnityEngine.Random;
using MysteryChance = WeightedChanceParam;

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
		ESound sound = ESound.None;
		switch(pConfig.Type)
		{
			case EPowerUp.Health:
				pPlayer.Stats.AddHealth(20);
				sound = ESound.Item_Powerup_Heal;
				break;

			case EPowerUp.Ammo:
				pPlayer.WeaponController.OnPowerUpAmmo();
				sound = ESound.Item_Powerup_Ammo;
				break;

			case EPowerUp.Speed:
				pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.DoubleSpeed, SPEED_DURATION);//, SPEED_VALUE);
				sound = ESound.Item_Powerup_Speed;
				break;

			case EPowerUp.Mystery:
				return HandleMystery(pPlayer);

			case EPowerUp.Shield:
				pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.Shield, SHIELD_DURATION);
				sound = ESound.Item_Powerup_Shield;
				break;

			default:
				Debug.LogError($"Powerup {pConfig.Type} not handled!");
				break;
		}
		pPlayer.PlaySound(sound);

		return pConfig.MapItemInfo.StatusText;
	}

	/// <summary>
	/// Randomly applies some type of powerup.
	/// Returns the text with outcome description.
	/// </summary>
	private static string HandleMystery(Player pPlayer)
	{
		//result of the mystery can be classic powerup, playerEffect or basically anything
		//so the handle is not very general

		EPowerUp powerUp = EPowerUp.None;
		string outcome = "";

		const int small_chance = 1;
		const int normal_chance = 2;
		const int high_chance = 3;

		MysteryChance resultAmmo = new MysteryChance(() => { powerUp = EPowerUp.Ammo; }, normal_chance);
		MysteryChance resultHealth = new MysteryChance(() => { powerUp = EPowerUp.Health; }, normal_chance);
		MysteryChance resultSpeed = new MysteryChance(() => { powerUp = EPowerUp.Speed; }, high_chance);
		MysteryChance resultShield = new MysteryChance(() => { powerUp = EPowerUp.Shield; }, high_chance);


		MysteryChance resultReceiveDamage = new MysteryChance(() =>
		{
			pPlayer.Stats.AddHealth(-20);
			pPlayer.PlaySound(ESound.Item_Powerup_ReceiveDamage);
			outcome = "- health";
		}, small_chance);

		MysteryChance resultHalfSpeed = new MysteryChance(() =>
		 {
			 pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.HalfSpeed, 5);
			 pPlayer.PlaySound(ESound.Item_Powerup_Slow);
			 outcome = "- speed";
		 }, small_chance);

		MysteryChance resultDoubleDamage = new MysteryChance(() =>
		{
			pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.DoubleDamage, 5);
			pPlayer.PlaySound(ESound.Item_Powerup_DoubleDamage);
			outcome = "+ damage";
		}, normal_chance);

		MysteryChance resultHalfDamage = new MysteryChance(() =>
		{
			pPlayer.Stats.StatsEffect.ApplyEffect(EPlayerEffect.HalfDamage, 5);
			pPlayer.PlaySound(ESound.Item_Powerup_HalfDamage);
			outcome = "- damage";
		}, small_chance);


		WeightedChanceExecutor weightedChanceExecutor =
			new WeightedChanceExecutor(
				resultAmmo,
				resultHealth,
				resultSpeed,
				resultShield,
				resultReceiveDamage,
				resultHalfSpeed,
				resultDoubleDamage,
				resultHalfDamage
			);

		weightedChanceExecutor.Execute();
		if(outcome.Length > 0)
			return outcome;


		return ApplyPowerup(Brainiacs.Instance.ItemManager.GetPowerupConfig(powerUp), pPlayer);
	}

}