using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class PlayerWeaponConfig : PlayerItemConfig
{
	public EWeaponId Id;

	public int ammo;
	public int Magazines;
	public int Cooldown;
	public float Cadency;

	public ProjectileConfig Projectile;
	public Sprite InfoSprite;

	public override void OnEnterPlayer(Player pPlayer)
	{
		pPlayer.ItemController.OnEnterWeapon(this);
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

	

	MP40 = 10,
	Flamethrower = 11,
	Lasergun = 12,



	Special_Einstein = 100,
	Special_Curie = 101,


	TestGun = 666,
	TestGun2 = 667,
	TestGun3 = 668,
}


