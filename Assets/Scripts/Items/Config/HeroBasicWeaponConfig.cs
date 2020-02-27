using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "HeroBasicWeapon", menuName = "ScriptableObjects/HeroBasicWeapon")]
public class HeroBasicWeaponConfig : WeaponConfig
{
	public InHandWeaponVisualInfo VisualInfo;

	//hero basic weapons only differ in visual
	public ProjectileVisual ProjectileVisual;
}

