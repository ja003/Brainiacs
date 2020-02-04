using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hero", menuName = "ScriptableObjects/Hero")]
public class HeroConfig : ScriptableObject
{
	public EHero hero;

	public EWeaponId defaultWeapon;
	//todo: special weapon own class?
	public EWeaponId specialWeapon;
}
