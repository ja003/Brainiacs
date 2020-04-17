using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just spawns mine. The explosion logic is in SpecialNobelMine
/// </summary>
public class SpecialNobel : PlayerWeaponSpecialController
{
	[SerializeField] SpecialNobelMine minePrefab = null;

	protected override void OnInit()
	{
	}


	protected override void OnUse()
	{
		//Spawning is handled by local player
		if(!Photon.IsMine)
		{
			//Debug.Log("I dont spawn mine");
			return;
		}

		SpecialNobelMine mineInstance =
			InstanceFactory.Instantiate(minePrefab.gameObject, weaponContoller.GetProjectileStart().position).GetComponent<SpecialNobelMine>();

		mineInstance.Spawn(Owner);
	}


	protected override void OnStopUse()
	{
	}
}
