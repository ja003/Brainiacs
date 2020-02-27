using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : GameController
{
	[SerializeField]
	private Projectile prefab;
	
	public void SpawnProjectile(Vector3 pPosition, Player pOwner, ProjectileConfig pConfig)
	{
		Projectile newProjectile = Instantiate(prefab, pPosition, Quaternion.identity, GetHolder());

		newProjectile.Spawn(pOwner, pConfig);
	}

	protected override void OnGameActivated()
	{
	}

	protected override void OnGameAwaken()
	{
	}
}
