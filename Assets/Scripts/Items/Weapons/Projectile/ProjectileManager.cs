using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
	[SerializeField]
	private Projectile prefab;

	[SerializeField]
	private Transform holder;

	public void SpawnProjectile(Vector3 pPosition, PlayerMovement pPlayerMovement, ProjectileConfig pConfig)
	{
		Projectile newProjectile = Instantiate(prefab, pPosition, Quaternion.identity, holder);

		newProjectile.Spawn(pPlayerMovement.CurrentDirection, pConfig, pPlayerMovement.PlayerCollider);
	}
}
