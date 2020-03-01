using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MapObject
{
	MapWeaponConfig weaponConfig; //todo: maybe id is enough?
	PowerUpConfig powerUpConfig;

	private void Spawn(Vector3 pPosition)
	{
		transform.position = pPosition;
		//Debug.Log($"Item {config} spawned at {pPosition}");
	}

	public void Spawn(Vector3 pPosition, MapWeaponConfig pConfig)
	{
		transform.position = pPosition;
		weaponConfig = pConfig;
		spriteRend.sprite = pConfig.MapItemInfo.MapSprite;
		Spawn(pPosition);
	}

	public void Spawn(Vector3 pPosition, PowerUpConfig pConfig)
	{
		transform.position = pPosition;
		powerUpConfig = pConfig;
		spriteRend.sprite = pConfig.MapItemInfo.MapSprite;
		Spawn(pPosition);
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
		if(powerUpConfig != null)
		{
			PowerupManager.HandlePowerup(powerUpConfig, pPlayer);
		}
		else if(weaponConfig != null)
		{
			pPlayer.ItemController.AddMapWeapon(weaponConfig.Id);
		}
		//TODO: special weapon + handle error

		ReturnToPool();
	}

	private void ReturnToPool()
	{
		gameObject.SetActive(false);
	}

	protected override void OnCollisionEffect(Projectile pProjectile)
	{
		if(weaponConfig != null)
		{
			Debug.Log("todo: weapon explode");
		}
		ReturnToPool();
	}
}
