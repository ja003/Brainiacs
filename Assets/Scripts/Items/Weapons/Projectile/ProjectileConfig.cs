using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/Projectile")]
public class ProjectileConfig : ScriptableObject
{
	public ProjectileVisual Visual;
	public int Damage;
	public float Speed;
	public float Dispersion;
}

