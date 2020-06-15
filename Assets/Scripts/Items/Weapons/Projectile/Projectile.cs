using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Projectile : PoolObjectNetwork, ITeleportable
{

	public Vector2 Direction { get; private set; }
	public ProjectileConfig config;

	bool isInited;

	//[SerializeField] public ProjectilePhoton Photon;

	public Projectile LocalImage;

	public Player Owner { get; private set; }

	protected override void OnSetActive0(bool pValue)
	{
		//Debug.Log("projectile OnSetActive " + pValue);
		spriteRend.enabled = pValue;
		animator.enabled = pValue;
		boxCollider2D.enabled = pValue;
	}

	/// <summary>
	/// Called only at owner side
	/// </summary>
	public void Spawn(Player pOwner, ProjectileConfig pConfig, EDirection pDirection = EDirection.None)
	{
		Owner = pOwner;
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

	/// <summary>
	/// Called on owner and image side
	/// </summary>
	public void SetSpawn(Vector2 pProjectileDirection, EWeaponId pId, EDirection pPlayerDirection)
	{
		SetActive(true); //has to be called before animator.SetFloat

		//projectile type is based on weapon
		//their condition in animator must match the weapon id
		const string ANIM_KEY_WEAPON = "weapon";
		animator.SetFloat(ANIM_KEY_WEAPON, (int)pId);

		config = brainiacs.ItemManager.GetProjectileConfig(pId);

		boxCollider2D.size = config.Visual.GetCollider().size;
		boxCollider2D.offset = config.Visual.GetCollider().offset;
		boxCollider2D.enabled = false;

		Direction = pProjectileDirection;

		transform.Rotate(Utils.GetRotation(pPlayerDirection, 180));

		isInited = true;

		game.ProjectileManager.RegisterProjectile(this);

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
		if(!isInited)
			return;

		//transform.position += Utils.GetVector(direction) *
		transform.position += new Vector3(Direction.x, Direction.y, 0) * Time.fixedDeltaTime * config.Speed;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(!isInited)
		{
			Debug.LogError("OnTriggerEnter2D shouldn be called before init");
			return;
		}

		//Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);

		ICollisionHandler handler =
			collision.gameObject.GetComponent<ICollisionHandler>();

		bool result = false;
		if(handler != null)
			result = handler.OnCollision(config.Damage, Owner, gameObject);

		if(result)
			ReturnToPool();
		//Photon.Destroy();
		//ReturnToPool();
	}

	public override string ToString()
	{
		return $"Projectile {config.WeaponId}, of {Owner}";
	}

	protected override void OnReturnToPool2()
	{
		game.ProjectileManager.OnDestroyProjectile(this);
	}

	/// TELEPORT

	public ITeleportable TeleportTo(Teleport pTeleport)
	{
		//simulated on owner side
		if(!Owner.IsItMe)
			return null;

		Projectile newProjectile = game.ProjectileManager.SpawnProjectile(
			pTeleport.GetOutPosition(), Owner, config, pTeleport.OutDirection);
		ReturnToPool();
		return newProjectile;
	}
}
