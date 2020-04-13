using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Projectile : BrainiacsBehaviour
{

	private Vector3 direction;
	public ProjectileConfig config;

	bool inited;

	[FormerlySerializedAs("Network")]
	[SerializeField] public ProjectilePhoton Photon;

	public Projectile LocalImage;

	Player owner;

	public void Spawn(Player pOwner, ProjectileConfig pConfig, EDirection pDirection = EDirection.None)
	{
		owner = pOwner;
		//Network.Init(this);

		//projectile type is based on weapon
		//their condition in animator must match the weapon id
		//animator.SetFloat(ANIM_KEY_WEAPON, (int)pConfig.WeaponId);

		//owner == null => it is remote projectile and has to have direction set
		if(pDirection == EDirection.None && !pOwner)
		{
			Debug.LogError("Projectile doesnt have owner or direction set");
		}

		//if direction is not specified, use players current direction
		EDirection direction = pDirection == EDirection.None ?
			pOwner.Movement.CurrentDirection : pDirection;

		Vector2 projectileDirection = GetDirectionVector(direction, pConfig.Dispersion);
		SetSpawn(projectileDirection, pConfig.WeaponId, direction);

		//config = pConfig;
		boxCollider2D.enabled = true;

		//pOwner => local projectile, else => remote -> dont detect collisions
		if(pOwner)
		{
			Physics2D.IgnoreCollision(GetComponent<Collider2D>(), pOwner.Movement.PlayerCollider);
			//todo: call UpdateOrderInLayer after network message is received?
			UpdateOrderInLayer(pOwner);
		}
		else
		{
			boxCollider2D.enabled = false;
		}


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

		Photon.Send(EPhotonMsg.Projectile_Spawn, pProjectileDirection, pId, pPlayerDirection);
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

	//private void OnCollisionEnter2D(Collision2D collision)
	//{
	//	ICollisionHandler handler =
	//		collision.collider.GetComponent<ICollisionHandler>();

	//	bool result = false;
	//	if(handler != null)
	//		result = handler.OnCollision(config.Damage);

	//	if(result)
	//		Network.Destroy();
	//		//ReturnToPool();
	//}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		ICollisionHandler handler =
			collision.gameObject.GetComponent<ICollisionHandler>();

		bool result = false;
		if(handler != null)
			result = handler.OnCollision(config.Damage, owner);

		if(result)
			Photon.Destroy();
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
