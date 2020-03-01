using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// NOTE: projectile animations are binded to weapon id. 
/// If id changes, change value in AC_Projectile
/// </summary>
public enum EWeaponId
{
	None,

	//map projectile
	MP40 = 10,
	Lasergun = 11,
	//todo: biogun spawns AOE DOT field
	Biogun = 12,

	//map special
	Flamethrower = 50,
	Mine = 51,



	//None = 0,
	//Currie = 1,
	//DaVinci = 2,
	//Einstein = 3,
	//Tesla = 4,
	//Nobel = 5,
	Special_None = 100,
	Special_Curie = 101,
	Special_DaVinci = 102,
	Special_Einstein = 103,
	Special_Nobel = 104,
	Special_Tesla = 105,


	Basic_None = 200,
	Basic_Curie = 201,
	Basic_DaVinci = 202,
	Basic_Einstein = 203,
	Basic_Nobel = 204,
	Basic_Tesla = 205,


	TestGun = 666,
	TestGun2 = 667,
	TestGun3 = 668,
}


