using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : GameController
{
	[SerializeField] private Projectile prefab = null;

	public int LayerProjectile => prefab.gameObject.layer;

	//todo: is needed?
	//public void SpawnProjectile(Vector2 pPosition, Player pOwner, EWeaponId pWeapon, EDirection pDirection = EDirection.None)
	//{
	//	SpawnProjectile(pPosition, pOwner, brainiacs.ItemManager.GetProjectileConfig(pWeapon), pDirection);
	//}

	//TODO: implement efficient storage of projectiles.
	//NOTE: in MP all sides generate projectiles!
	public List<Projectile> ActiveProjectiles = new List<Projectile>();

	public void RegisterProjectile(Projectile pProjectile)
	{
		//Debug.Log("Add projectile " + pProjectile);
		ActiveProjectiles.Add(pProjectile);
	}
	public void OnDestroyProjectile(Projectile pProjectile)
	{
		//Debug.Log("Remove projectile " + pProjectile);
		ActiveProjectiles.Remove(pProjectile);
	}

	public Projectile SpawnProjectile(Vector2 pPosition, Player pOwner, ProjectileConfig pConfig, EDirection pDirection = EDirection.None)
	{
		Projectile newProjectile = InstanceFactory.Instantiate(prefab.gameObject, pPosition).GetComponent<Projectile>();

		newProjectile.Spawn(pOwner, pConfig, pDirection);
		return newProjectile;
	}

	//int lastProjectileId;
	//public int GetNextProjectileId()
	//{
	//	lastProjectileId++;
	//	return lastProjectileId;
	//}

	protected override void OnMainControllerActivated()
	{
	}

	protected override void OnMainControllerAwaken()
	{

	}
}
