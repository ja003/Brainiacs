using FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MapObject
{
	MapWeaponConfig weaponConfig;
	MapSpecialWeaponConfig weaponSpecialConfig;
	PowerUpConfig powerUpConfig;

	[SerializeField] SpriteRenderer explosion;

	//[SerializeField] MapItemPhoton photon;

	protected override void OnSetActive0(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue;
		circleCollider2D.enabled = false; //enabled only on explosion
		explosion.enabled = false;
	}

	bool isSpawned;
	//bool isMine;

	private void Spawn(Vector2 pPosition)
	{
		SetActive(true);
		transform.position = pPosition;
		//Debug.Log($"Item spawned at {pPosition}");

		game.Map.Items.RegisterItem(this);
		isSpawned = true;
		boxCollider2D.enabled = true;
		//isMine = brainiacs.PhotonManager.IsMaster();

		animator.Rebind();

		Collider2D col;
		if(col = Physics2D.OverlapBox(pPosition, boxCollider2D.size, 0, game.Layers.MapObject))
		{
			Debug.LogError("OVERLAPS " + pPosition + " - " + col.gameObject.name);
		}
	}

	public void Init(Vector2 pPosition, PowerUpConfig pPowerUp)
	{
		powerUpConfig = pPowerUp;
		spriteRend.sprite = pPowerUp.MapItemInfo.MapSprite;
		Spawn(pPosition);

		Photon.Send(EPhotonMsg.MapItem_InitPowerUp, pPosition, pPowerUp.Type);
	}

	public void Init(Vector2 pPosition, MapWeaponConfig pWeapon)
	{
		weaponConfig = pWeapon;
		spriteRend.sprite = pWeapon.MapItemInfo.MapSprite;
		Spawn(pPosition);

		Photon.Send(EPhotonMsg.MapItem_InitMapBasic, pPosition, pWeapon.Id);

	}

	public void Init(Vector2 pPosition, MapSpecialWeaponConfig pSpecialWeapon)
	{
		weaponSpecialConfig = pSpecialWeapon;
		spriteRend.sprite = pSpecialWeapon.MapItemInfo.MapSprite;
		Spawn(pPosition);

		Photon.Send(EPhotonMsg.MapItem_InitMapSpecial, pPosition, pSpecialWeapon.Id);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(!isSpawned)
			return;

		Player player = collision.gameObject.GetComponent<Player>();
		if(player && player.IsItMe)
		{
			OnEnter(player);
		}
	}

	private void OnEnter(Player pPlayer)
	{
		if(powerUpConfig != null)
		{
			//Debug.Log("OnEnter powerup");
			PowerupManager.HandlePowerup(powerUpConfig, pPlayer);
		}
		else if(weaponConfig != null)
		{
			//Debug.Log("OnEnter weapon");
			pPlayer.ItemController.AddMapWeapon(weaponConfig.Id);
		}
		else if(weaponSpecialConfig != null)
		{
			//Debug.Log("OnEnter weapon special");
			pPlayer.ItemController.AddMapWeaponSpecial(weaponSpecialConfig.Id);
		}
		//TODO: special weapon + handle error

		ReturnToPool();
	}

	/// <summary>
	/// When item is hit it explodes
	/// </summary>
	private void Explode()
	{
		spriteRend.enabled = false;
		explosion.enabled = true;
		animator.SetBool("explode", true);
		DoInTime(ApplyExplosion, 0.1f);
	}

	private void ApplyExplosion()
	{
		Debug.Log("ApplyExplosion");
		circleCollider2D.enabled = true;
		List<Collider2D> hitResult = new List<Collider2D>();
		circleCollider2D.OverlapCollider(new ContactFilter2D(), hitResult);
		foreach(var hit in hitResult)
		{
			ICollisionHandler handler = hit.GetComponent<ICollisionHandler>();
			if(handler != null)
			{
				handler.OnCollision(30, null, gameObject);
			}
			//Debug.Log(hit.gameObject.name);
		}

	}

	/// <summary>
	/// After explosion it is returned to pool
	/// </summary>
	public void OnExplosionFinished()
	{
		ReturnToPool();
	}

	protected override void OnReturnToPool2()
	{
		game.Map.Items.OnDestroyItem(this);
		base.OnReturnToPool2();
	}

	protected override void OnCollisionEffect(int pDamage, GameObject pOrigin)
	{
		if(!isSpawned)
			return;

		if(pDamage <= 0)
			return;

		Explode();
	}
}
