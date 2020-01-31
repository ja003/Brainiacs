using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class PlayerWeaponConfig : PlayerItemConfig
{
	public EWeaponId Id;

	public int damage;
	public int ammo;
	public int Cooldown;

	public override void OnEnterPlayer(Player pPlayer)
	{
		pPlayer.ItemHandler.OnEnterWeapon(this);
	}

	public override string ToString()
	{
		return $"Weapon {Id}";
	}
}


public enum EWeaponId
{
	None,

	TestGun,
	TestGun2,
	TestGun3,
}


