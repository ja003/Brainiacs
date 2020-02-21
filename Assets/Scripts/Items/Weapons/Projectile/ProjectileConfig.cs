﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/Projectile")]
public class ProjectileConfig : ScriptableObject
{
	public Sprite sprite;
	public int damage;
	public float speed;
	public float Dispersion;
	public Vector2 ColliderOffset;
	public Vector2 ColliderSize;
}
