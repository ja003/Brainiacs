﻿using ExitGames.Client.Photon;
using FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MapObject
{
	//todo: refactor into multiple object types?
	//or some RewardHandler?
	//MapWeaponConfig weaponConfig;
	//MapSpecialWeaponConfig weaponSpecialConfig;
	//PowerUpConfig powerUpConfig;

	[SerializeField] SpriteRenderer explosion;
	[SerializeField] float explosionPushForce;

	//[SerializeField] MapItemPhoton photon;

	protected override void OnSetActive0(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue;
		circleCollider2D.enabled = false; //enabled only on explosion
		explosion.enabled = false;
	}

	bool isSpawned;

	private EType type;
	private int subtypeIndex = -1;

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

	public enum EType
	{
		None,
		Weapon,
		SpecialWeapon,
		PowerUp,
		GameEffect
	}

	public void Init(Vector2 pPosition, EType pType, int pSubtypeIndex)
	{
		type = pType;
		subtypeIndex = pSubtypeIndex;
		Spawn(pPosition);
		RefreshMapSprite();
		Photon.Send(EPhotonMsg.MapItem_Init, pPosition, pType, pSubtypeIndex);
	}

	private void RefreshMapSprite()
	{
		Sprite s = null;
		switch(type)
		{
			case EType.Weapon:
				MapWeaponConfig weapon = GetMapWeaponConfig();
				s = weapon.MapItemInfo.MapSprite;
				break;
			case EType.SpecialWeapon:
				MapSpecialWeaponConfig specialWeapon = GetMapSpecialWeaponConfig();
				s = specialWeapon.MapItemInfo.MapSprite;
				break;
			case EType.PowerUp:
				PowerUpConfig powerUpConfig = GetPowerUpConfig();
				s = powerUpConfig.MapItemInfo.MapSprite;
				break;

			case EType.GameEffect:
				GameEffectConfig config = GetGameEffectConfig();
				s = config.MapItemInfo.MapSprite;
				break;

			default:
				Debug.LogError("Map item type not inited");
				break;
		}

		spriteRend.sprite = s;
	}

	private MapSpecialWeaponConfig GetMapSpecialWeaponConfig()
	{
		return brainiacs.ItemManager.GetMapSpecialWeaponConfig((EWeaponId)subtypeIndex);
	}

	private MapWeaponConfig GetMapWeaponConfig()
	{
		return brainiacs.ItemManager.GetMapWeaponConfig((EWeaponId)subtypeIndex);
	}

	private PowerUpConfig GetPowerUpConfig()
	{
		return brainiacs.ItemManager.GetPowerupConfig((EPowerUp)subtypeIndex);
	}

	private GameEffectConfig GetGameEffectConfig()
	{
		return brainiacs.ItemManager.GetGameEffectConfig((EGameEffect)subtypeIndex);
	}

	//public void Init(Vector2 pPosition, PowerUpConfig pPowerUp)
	//{
	//	powerUpConfig = pPowerUp;
	//	spriteRend.sprite = pPowerUp.MapItemInfo.MapSprite;
	//	Spawn(pPosition);

	//Photon.Send(EPhotonMsg.MapItem_InitPowerUp, pPosition, pPowerUp.Type);
	//}

	//public void Init(Vector2 pPosition, MapWeaponConfig pWeapon)
	//{
	//	weaponConfig = pWeapon;
	//	spriteRend.sprite = pWeapon.MapItemInfo.MapSprite;
	//	Spawn(pPosition);

	//	Photon.Send(EPhotonMsg.MapItem_InitMapBasic, pPosition, pWeapon.Id);

	//}

	//public void Init(Vector2 pPosition, MapSpecialWeaponConfig pSpecialWeapon)
	//{
	//	weaponSpecialConfig = pSpecialWeapon;
	//	spriteRend.sprite = pSpecialWeapon.MapItemInfo.MapSprite;
	//	Spawn(pPosition);

	//	Photon.Send(EPhotonMsg.MapItem_InitMapSpecial, pPosition, pSpecialWeapon.Id);
	//}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Debug.Log("OnTriggerEnter2D " + isSpawned);
		if(!isSpawned)
			return;

		Player player = collision.gameObject.GetComponent<Player>();
		//Debug.Log("player " + player?.IsItMe);
		if(player && player.IsItMe)
		{
			OnEnter(player);
		}
	}

	private void OnEnter(Player pPlayer)
	{
		switch(type)
		{
			case EType.Weapon:
				MapWeaponConfig weapon = GetMapWeaponConfig();
				pPlayer.ItemController.AddMapWeapon(weapon.Id);
				break;
			case EType.SpecialWeapon:
				MapSpecialWeaponConfig specialWeapon = GetMapSpecialWeaponConfig();
				pPlayer.ItemController.AddMapWeaponSpecial(specialWeapon.Id);
				break;
			case EType.PowerUp:
				PowerUpConfig powerUpConfig = GetPowerUpConfig();
				PowerupManager.HandlePowerup(powerUpConfig, pPlayer);
				break;
			case EType.GameEffect:
				GameEffectConfig config = GetGameEffectConfig();
				game.GameEffect.HandleEffect(config.Type);
				break;
			default:
				Debug.LogError("Map item type not handled");
				break;
		}
		PlaySound(ESound.Item_Weapon_Pickup);

		//if(powerUpConfig != null)
		//{
		//	//Debug.Log("OnEnter powerup");
		//	PowerupManager.HandlePowerup(powerUpConfig, pPlayer);
		//}
		//else if(weaponConfig != null)
		//{
		//	//Debug.Log("OnEnter weapon");
		//	pPlayer.ItemController.AddMapWeapon(weaponConfig.Id);
		//	PlaySound(ESound.Item_Weapon_Pickup);
		//}
		//else if(weaponSpecialConfig != null)
		//{
		//	//Debug.Log("OnEnter weapon special");
		//	pPlayer.ItemController.AddMapWeaponSpecial(weaponSpecialConfig.Id);
		//	PlaySound(ESound.Item_Weapon_Pickup);
		//}
		//TODO: special weapon + handle error

		ReturnToPool();
	}

	/// <summary>
	/// When item is hit it explodes
	/// </summary>
	private void Explode()
	{
		DoExplosionEffect(false);
		DoInTime(ApplyExplosion, 0.1f);
	}

	/// <summary>
	/// Visual and soudn effect.
	/// pIsRPC = Is called from RPC => dont send another message.
	/// </summary>
	public void DoExplosionEffect(bool pIsRPC)
	{
		spriteRend.enabled = false;
		explosion.enabled = true;
		animator.SetBool("explode", true);
		PlaySound(ESound.Item_Explode);
		if(!pIsRPC)
			Photon.Send(EPhotonMsg.MapItem_DoExplosionEffect);
	}

	private void ApplyExplosion()
	{
		//Debug.Log("ApplyExplosion");
		circleCollider2D.enabled = true;
		List<Collider2D> hitResult = new List<Collider2D>();
		circleCollider2D.OverlapCollider(new ContactFilter2D(), hitResult);
		foreach(var hit in hitResult)
		{
			ICollisionHandler handler = hit.GetComponent<ICollisionHandler>();
			if(handler != null)
			{
				Vector3 push = (hit.transform.position - transform.position).normalized * explosionPushForce;
				handler.OnCollision(30, null, gameObject, push);
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
		type = EType.None;
		subtypeIndex = -1;

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
