using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class SpecialCurie : PlayerWeaponSpecialController
{
	[SerializeField] SpecialCurieTruck truckPrefab = null;
	[SerializeField] ProjectileConfig projectile = null;

	protected override void Awake()
	{
		//projectile has to be registered so we know its config.
		//it is the same as Curie basic (but can be changed in P_Projectile animator and config)
		brainiacs.ItemManager.AddProjectile(EWeaponId.Special_Curie, projectile);

		base.Awake();

	}

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

		SpecialCurieTruck truckInstance =
			InstanceFactory.Instantiate(truckPrefab.gameObject, weaponContoller.GetProjectileStart().position).GetComponent<SpecialCurieTruck>();

		truckInstance.Spawn(Owner, projectile);
	}

	protected override void OnStopUse()
	{
	}

	protected override void OnSetActive(bool pValue)
	{
	}
}
