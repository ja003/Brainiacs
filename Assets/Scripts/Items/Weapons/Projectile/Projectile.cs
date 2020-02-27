using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : GameBehaviour
{
	//private EDirection direction;
	private Vector3 direction;
	private ProjectileConfig config;

	bool inited;

	public void Spawn(Player pOwner, ProjectileConfig pConfig)
	{
		spriteRend.sprite = pConfig.Visual.GetSprite();
		EDirection playerDir = pOwner.Movement.CurrentDirection;
		direction = GetDirectionVector(playerDir, pConfig.Dispersion);
		config = pConfig;
		inited = true;

		boxCollider2D.size = pConfig.Visual.GetCollider().size;
		boxCollider2D.offset = pConfig.Visual.GetCollider().offset;

		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), pOwner.Movement.PlayerCollider);

		UpdateOrderInLayer(pOwner);

		transform.Rotate(Utils.GetRotation(playerDir, 180));
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
		//todo: make general class DamageHandler
		//for players, map objects...
		Player player = collision.collider.GetComponent<Player>();
		if(player)
		{
			OnEnter(player);
		}
	}

	private void OnEnter(Player pPlayer)
	{
		pPlayer.Stats.AddHealth(-config.Damage);
		//todo: return to pool
		gameObject.SetActive(false);
		inited = false;
	}

	
}
