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

	//edit: InHandWeaponInfo added to HeroBasicWeaponConfig
	//public InHandWeaponInfo(bool pHeroBasic) : this()
	//{
	//	if(pHeroBasic)
	//	{
	//		Ammo = 15;
	//		Magazines = 666;
	//		Cooldown = 1;
	//		Cadency = 0.2f;
	//	}
	//}
}

[Serializable]
public struct InHandWeaponVisualInfo
{
	public Sprite PlayerSpriteUp;
	public Sprite PlayerSpriteRight;
	public Sprite playerSpriteDown;
	public Sprite PlayerSpriteLeft;

	public bool DisableHandsUp;
	public bool DisableHandsRight;
	public bool DisableHandsDown;
	public bool DisableHandsLeft;

	public Sprite InfoSprite;
}

//todo: implement rest like this?
[Serializable]
public struct MapItemInfo
{
	public Sprite MapSprite;
	public Sprite StatusSprite; //icon that will appear after item is picked up
	public string StatusText;
}

[Serializable]
public struct SpecialWeaponInfo
{
	public InHandWeaponInfo InHandInfo;
	//public PlayerWeaponSpecialController ControllerPrefab;
	public PlayerWeaponSpecialPrefab Prefab;
}


[Serializable]
public struct ProjectileWeaponInfo
{
	public ProjectileConfig Projectile;

	[SerializeField] Vector2 ProjectileStartUpOffset;
	[SerializeField] Vector2 ProjectileStartRightOffset;
	[SerializeField] Vector2 ProjectileStartDownOffset;
	[SerializeField] Vector2 ProjectileStartLeftOffset;

	public Vector2 GetProjectileStartOffset(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return ProjectileStartUpOffset;
			case EDirection.Right:
				return ProjectileStartRightOffset;
			case EDirection.Down:
				return ProjectileStartDownOffset;
			case EDirection.Left:
				return ProjectileStartLeftOffset;
		}
		Debug.LogError("invalid direction");
		return Vector2.zero;
	}

	/// <summary>
	/// Hero basic projectile info is in hero weapon config
	/// </summary>
	public ProjectileWeaponInfo(HeroBasicWeaponConfig pConfig) : this()
	{
		Projectile = pConfig.ProjectileWeaponInfo.Projectile;

		//CHANGE: hero projectiles may differ

		//Projectile.Visual = pConfig.ProjectileVisual;

		//Projectile.Damage = 5;
		//Projectile.Dispersion = 0.5f;
		//Projectile.Speed = 2;

		//Projectile.WeaponId = pConfig.Id;
	}
}
