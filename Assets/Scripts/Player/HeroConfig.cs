using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hero", menuName = "ScriptableObjects/Hero")]
public class HeroConfig : ScriptableObject
{
	public EHero Hero;

	public HeroBasicWeaponConfig BasicWeapon;
	public HeroSpecialWeaponConfig SpecialWeapon;

	public Sprite Portrait;
}
