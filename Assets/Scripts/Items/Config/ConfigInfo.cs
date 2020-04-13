using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct InHandWeaponInfo
{
	public int Ammo;
	public int Magazines;
	public float Cooldown;
	public float Cadency;

	public InHandWeaponInfo(bool pHeroBasic) : this()
	{
		if(pHeroBasic)
		{
			Ammo = 15;
			Magazines = 666;
			Cooldown = 1;
			Cadency = 0.2f;
		}
	}
}

[Serializable]
public struct InHandWeaponVisualInfo
{
	public Sprite PlayerSpriteUp;
	public Sprite PlayerSpriteRight;
	public Sprite playerSpriteDown;
	public Sprite PlayerSpriteLeft;

	public Sprite InfoSprite;
}

//todo: implement rest like this?
[Serializable]
public struct MapItemInfo
{
	public Sprite MapSprite;
	public Sprite StatusSprite; //icon that will apear after item is picked up
	public string StatusText;
}

[Serializable]
public struct SpecialWeaponInfo
{
	[FormerlySerializedAs("InHandWeaponlInfo")]
	public InHandWeaponInfo InHandInfo;
	public PlayerWeaponSpecialController ControllerPrefab;
}


[Serializable]
public struct ProjectileWeaponInfo
{
	public ProjectileConfig Projectile;

	/// <summary>
	/// Hero basic projectile info is in hero weapon config
	/// </summary>
	public ProjectileWeaponInfo(HeroBasicWeaponConfig pConfig) : this()
	{
		Projectile = pConfig.Projectile;

		//CHANGE: hero projectiles may differ

		//Projectile.Visual = pConfig.ProjectileVisual;

		//Projectile.Damage = 5;
		//Projectile.Dispersion = 0.5f;
		//Projectile.Speed = 2;

		//Projectile.WeaponId = pConfig.Id;
	}
}
