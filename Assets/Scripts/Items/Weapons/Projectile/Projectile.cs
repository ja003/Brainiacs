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

		transform.Rotate(GetRotation(pDirection));
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
		pPlayer.Health.OnHitBy(config);
		//todo: return to pool
		gameObject.SetActive(false);
		inited = false;
	}

	/// <summary>
	/// Projectile textures are oriented to the left.
	/// </summary>
	private static Vector3 GetRotation(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return new Vector3(0, 0, -90);
			case EDirection.Right:
				return new Vector3(0, 0, 180);
			case EDirection.Down:
				return new Vector3(0, 0, 90);
			case EDirection.Left:
				return Vector3.zero;
		}
		return Vector3.zero;
	}
}
