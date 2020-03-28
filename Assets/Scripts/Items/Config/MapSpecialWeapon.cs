using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSpecialWeapon", menuName = "ScriptableObjects/MapSpecialWeapon")]
public class MapSpecialWeapon : WeaponConfig
{
	public MapItemInfo MapItemInfo;
	public SpecialWeaponInfo SpecialWeaponInfo;
	public InHandWeaponInfo InHandInfo;
}