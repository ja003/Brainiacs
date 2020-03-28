using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : GameController
{
	[SerializeField]
	private Projectile prefab;

	public void SpawnProjectile(Vector3 pPosition, Player pOwner, ProjectileConfig pConfig)
	{
		Projectile newProjectile = PhotonNetwork.Instantiate(
			prefab.name, pPosition, Quaternion.identity).GetComponent<Projectile>();

		if(DebugData.LocalRemote)
		{
			Projectile localRemoteProjectile = PhotonNetwork.Instantiate(
				prefab.name, pPosition + Vector3.up, Quaternion.identity).GetComponent<Projectile>();
			newProjectile.LocalRemote = localRemoteProjectile;
		}

		newProjectile.Spawn(pOwner, pConfig);
	}

	protected override void OnMainControllerActivated()
	{
	}

	protected override void OnMainControllerAwaken()
	{
	}
}
