using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "ScriptableObjects/PowerUp", order = 2)]
public class PowerUpConfig : PlayerItemConfig
{
	public EPowerUp Id;
	public float Value;

	public override void OnEnterPlayer(Player pPlayer)
	{
		pPlayer.ItemHandler.OnEnterPowerUp(this);
	}

	public override string ToString()
	{
		return $"PowerUp {Id}";
	}
}

public enum EPowerUp
{
	None,

	Random,
	Health,
	Speed
}