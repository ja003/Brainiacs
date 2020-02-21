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

	public void Spawn(EDirection pDirection, ProjectileConfig pConfig, Collider2D pPlayerCollider)
	{
		spriteRend.sprite = pConfig.sprite;
		direction = GetDirectionVector(pDirection, pConfig.Dispersion);
		config = pConfig;
		inited = true;
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), pPlayerCollider);

		transform.Rotate(Utils.GetRotation(pDirection, 180));
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
		transform.position += direction * Time.deltaTime * config.speed;
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
		pPlayer.Stats.AddHealth(-config.damage);
		//todo: return to pool
		gameObject.SetActive(false);
		inited = false;
	}

	
}
