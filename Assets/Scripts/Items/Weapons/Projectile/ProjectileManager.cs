using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : GameController
{
	[SerializeField]
	private Projectile prefab;
	
	public void SpawnProjectile(Vector3 pPosition, PlayerMovement pPlayerMovement, ProjectileConfig pConfig)
	{
		Projectile newProjectile = Instantiate(prefab, pPosition, Quaternion.identity, GetHolder());

		newProjectile.Spawn(pPlayerMovement.CurrentDirection, pConfig, pPlayerMovement.PlayerCollider);
	}

	protected override void OnGameActivated()
	{
	}

	protected override void OnGameAwaken()
	{
	}
}
