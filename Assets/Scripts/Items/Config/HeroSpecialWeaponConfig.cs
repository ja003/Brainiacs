﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "HeroSpecialWeaponConfig", menuName = "ScriptableObjects/HeroSpecialWeaponConfig")]
public class HeroSpecialWeaponConfig : WeaponConfig
{
	public InHandWeaponInfo InHandInfo;
	public SpecialWeaponInfo SpecialWeaponInfo;
}

