﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/Projectile")]
public class ProjectileConfig : ScriptableObject
{
	public EWeaponId WeaponId;
	public ProjectileVisual Visual;
	public int Damage; //public new int Damage;
	public float Speed;
	public float Dispersion;
	public float PushForce;
}



//todo: hero projectiles are same for now, but they might change in order to balance heroes

//[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/Hero basic projectile")]
//public class HeroBasicProjectileConfig : ScriptableObject
//{
//	public EWeaponId WeaponId;
//	public ProjectileVisual Visual;

//	public int Damage => 5;
//	public float Speed => 2;
//	public float Dispersion => 0.5f;
//}