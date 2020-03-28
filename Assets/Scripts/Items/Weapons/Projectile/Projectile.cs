using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : BrainiacsBehaviour
{

	private Vector3 direction;
	public ProjectileConfig config;

	bool inited;

	[SerializeField] public ProjectileNetworkController Network;

	public Projectile LocalRemote;

	public void Spawn(Player pOwner, ProjectileConfig pConfig)
	{
		//Network.Init(this);

		//projectile type is based on weapon
		//their condition in animator must match the weapon id
		//animator.SetFloat(ANIM_KEY_WEAPON, (int)pConfig.WeaponId);

		EDirection playerDir = pOwner.Movement.CurrentDirection;
		Vector2 projectileDirection = GetDirectionVector(playerDir, pConfig.Dispersion);
		SetSpawn(projectileDirection, pConfig.WeaponId, playerDir);

		//config = pConfig;

		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), pOwner.Movement.PlayerCollider);

		UpdateOrderInLayer(pOwner);

		boxCollider2D.enabled = true;
	}

	public void SetSpawn(Vector3 pProjectileDirection, EWeaponId pId, EDirection pPlayerDirection)
	{
		//projectile type is based on weapon
		//their condition in animator must match the weapon id
		const string ANIM_KEY_WEAPON = "weapon";
		animator.SetFloat(ANIM_KEY_WEAPON, (int)pId);

		config = brainiacs.ItemManager.GetProjectileConfig(pId);

		boxCollider2D.size = config.Visual.GetCollider().size;
		boxCollider2D.offset = config.Visual.GetCollider().offset;
		boxCollider2D.enabled = false;

		direction = pProjectileDirection;

		transform.Rotate(Utils.GetRotation(pPlayerDirection, 180));

		inited = true;

		Network.Send(EPhotonMsg.Projectile_Spawn, pProjectileDirection, pId, pPlayerDirection);
	}

	private void UpdateOrderInLayer(Player pOwner)
	{
		int order = pOwner.Visual.GetProjectileSortOrder();
		spriteRend.sortingOrder = order;
		//Debug.Log("Set projectile order " + order);
	}

	private Vector2 GetDirectionVector(EDirection pDirection, float pDispersion)
	{
		Vector2 dir = Utils.GetVector2(pDirection);
		dir += Vector2.one * Random.Range(-pDispersion, pDispersion);
		return dir;
	}

	public void FixedUpdate()
	{
		if(!inited)
			return;

		//transform.position += Utils.GetVector(direction) *
		transform.position += direction * Time.deltaTime * config.Speed;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		IProjectileCollisionHandler handler =
			collision.collider.GetComponent<IProjectileCollisionHandler>();

		bool result = false;
		if(handler != null)
			result = handler.OnCollision(this);

		if(result)
			Network.Destroy();
			//ReturnToPool();
	}

	//TODO: pooling
	//private void ReturnToPool()
	//{
	//	PhotonNetwork.Destroy(Network.view);

	//	//gameObject.SetActive(false);
	//	//inited = false;
	//}


}
