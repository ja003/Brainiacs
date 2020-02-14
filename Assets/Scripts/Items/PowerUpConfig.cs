using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "ScriptableObjects/PowerUp", order = 2)]
public class PowerUpConfig : MapItemConfig
{
	[SerializeField]
	public EPowerUp type;

	public override void OnEnterPlayer(Player pPlayer)
	{
		PowerupManager.HandlePowerup(this, pPlayer);
	}

	public override string ToString()
	{
		return $"PowerUp {type}";
	}
}

public enum EPowerUp
{
	None,
	Health,
	Ammo,
	Speed,
	Mystery,
	Shield
}