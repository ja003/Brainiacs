using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Special Weapon", menuName = "ScriptableObjects/Special Weapon", order = 5)]
public class PlayerWeaponSpecialConfig : PlayerWeaponConfig
{
	public PlayerWeaponSpecialController controllerPrefab;

	public override bool IsSpecial()
	{
		return true;
	}

	//public override void SpecialInit(PlayerWeaponController pWeaponController)
	//{
	//	PlayerWeaponSpecialController instance = 
	//		Instantiate(controllerPrefab, pWeaponController.transform);
	//	instance.Init(pWeaponController);
	//}
}
