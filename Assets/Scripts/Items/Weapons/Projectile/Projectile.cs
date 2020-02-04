using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : GameBehaviour
{
	private EDirection direction;
	private ProjectileConfig config;

	bool inited;

	public void Spawn(EDirection pDirection, ProjectileConfig pConfig, Collider2D pPlayerCollider)
	{
		spriteRend.sprite = pConfig.sprite;
		direction = pDirection;
		config = pConfig;
		inited = true;
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), pPlayerCollider);

		transform.Rotate(GetRotation(pDirection));
	}


	public void FixedUpdate()
	{
		if(!inited)
			return;

		transform.position += Utils.GetVector(direction) * Time.deltaTime * config.speed;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
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
