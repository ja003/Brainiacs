using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class PlayerWeaponConfig : PlayerItemConfig
{
	public EWeaponId Id;

	public int damage;
	public int ammo;
	public int Magazines;
	public int Cooldown;

	public ProjectileConfig Projectile;
	public Sprite InfoSprite;

	public override void OnEnterPlayer(Player pPlayer)
	{
		pPlayer.ItemHandler.OnEnterWeapon(this);
	}

	public override string ToString()
	{
		return $"Weapon {Id}";
	}

	public virtual bool IsSpecial()
	{
		return false;
	}

	//public virtual void SpecialInit(PlayerWeaponController pWeaponController)
	//{
	//	Debug.LogError($"SpecialInit called on non-special weapon {Id}");
	//}
}


public enum EWeaponId
{
	None,

	TestGun,
	TestGun2,
	TestGun3,



	Special_Einstein = 100,
	Special_Curie = 101,
}


