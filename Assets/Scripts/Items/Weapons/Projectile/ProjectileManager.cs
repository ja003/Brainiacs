using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : GameController
{
	[SerializeField] private Projectile prefab = null;

	public int LayerProjectile => prefab.gameObject.layer;

	//todo: is needed?
	//public void SpawnProjectile(Vector3 pPosition, Player pOwner, EWeaponId pWeapon, EDirection pDirection = EDirection.None)
	//{
	//	SpawnProjectile(pPosition, pOwner, brainiacs.ItemManager.GetProjectileConfig(pWeapon), pDirection);
	//}

	public void SpawnProjectile(Vector3 pPosition, Player pOwner, ProjectileConfig pConfig, EDirection pDirection = EDirection.None)
	{
		Projectile newProjectile = InstanceFactory.Instantiate(prefab.gameObject, pPosition).GetComponent<Projectile>();

		newProjectile.Spawn(pOwner, pConfig, pDirection);
	}

	protected override void OnMainControllerActivated()
	{
	}

	protected override void OnMainControllerAwaken()
	{

	}
}
