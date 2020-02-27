using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MapWeapon", menuName = "ScriptableObjects/MapWeapon")]
public class MapWeaponConfig : WeaponConfig
{
	public MapItemInfo MapItemInfo;

	public InHandWeaponInfo InHandWeaponInfo;
	public InHandWeaponVisualInfo InHandWeaponVisualInfo;

	public ProjectileWeaponInfo ProjectileInfo;
}

