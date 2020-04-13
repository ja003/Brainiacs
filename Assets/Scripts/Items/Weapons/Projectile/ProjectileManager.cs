using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : GameController
{
	[SerializeField]
	private Projectile prefab;

	public int LayerProjectile => prefab.gameObject.layer;

	public void SpawnProjectile(Vector3 pPosition, Player pOwner, EWeaponId pWeapon, EDirection pDirection = EDirection.None)
	{
		SpawnProjectile(pPosition, pOwner, brainiacs.ItemManager.GetProjectileConfig(pWeapon), pDirection);
	}

	public void SpawnProjectile(Vector3 pPosition, Player pOwner, ProjectileConfig pConfig, EDirection pDirection = EDirection.None)
	{
		Projectile newProjectile = PhotonNetwork.Instantiate(
			prefab.name, pPosition, Quaternion.identity).GetComponent<Projectile>();

		//if(DebugData.LocalImage)
		//{
		//	Projectile localImageProjectile = PhotonNetwork.Instantiate(
		//		prefab.name, pPosition + Vector3.up, Quaternion.identity).GetComponent<Projectile>();
		//	newProjectile.LocalImage = localImageProjectile;
		//}

		newProjectile.Spawn(pOwner, pConfig, pDirection);
	}

	protected override void OnMainControllerActivated()
	{
	}

	protected override void OnMainControllerAwaken()
	{
			
	}
}
