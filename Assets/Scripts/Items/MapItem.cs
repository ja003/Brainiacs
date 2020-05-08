using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MapObject
{
	MapWeaponConfig weaponConfig;
	MapSpecialWeaponConfig weaponSpecialConfig;
	PowerUpConfig powerUpConfig;

	//[SerializeField] MapItemPhoton photon;

	protected override void OnSetActive(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue;
	}

	protected override void OnPhotonInstantiated()
	{
		
	}

	bool isSpawned;
	bool isMine;
	private void Spawn(Vector3 pPosition)
	{
		SetActive(true);
		transform.position = pPosition;
		//Debug.Log($"Item spawned at {pPosition}");

		game.Map.Items.RegisterItem(this);
		isSpawned = true;
		boxCollider2D.enabled = true;
		isMine = brainiacs.PhotonManager.IsMaster();
	}

	public void Init(Vector3 pPosition, PowerUpConfig pPowerUp)
	{
		powerUpConfig = pPowerUp;
		spriteRend.sprite = pPowerUp.MapItemInfo.MapSprite;
		Spawn(pPosition);

		Photon.Send(EPhotonMsg.MapItem_InitPowerUp, pPosition, pPowerUp.Type);
	}

	public void Init(Vector3 pPosition, MapWeaponConfig pWeapon)
	{
		weaponConfig = pWeapon;
		spriteRend.sprite = pWeapon.MapItemInfo.MapSprite;
		Spawn(pPosition);

		Photon.Send(EPhotonMsg.MapItem_InitMapBasic, pPosition, pWeapon.Id);

	}

	public void Init(Vector3 pPosition, MapSpecialWeaponConfig pSpecialWeapon)
	{
		weaponSpecialConfig = pSpecialWeapon;
		spriteRend.sprite = pSpecialWeapon.MapItemInfo.MapSprite;
		Spawn(pPosition);

		Photon.Send(EPhotonMsg.MapItem_InitMapSpecial, pPosition, pSpecialWeapon.Id);
	}


	//TODO: map item photon - sync visual

	//private void OnCollisionEnter2D(Collision2D collision)
	//{
	//	Player player = collision.collider.GetComponent<Player>();
	//	if(player)
	//	{
	//		OnEnter(player);
	//	}
	//}

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

	//public void ReturnToPool()
	//{
	//	if(isMine)
	//		InstanceFactory.Destroy(gameObject);
	//	else
	//		Photon.Send(EPhotonMsg.MapItem_ReturnToPool);
	//}

	private void OnDestroy()
	{
		if(!game)
			return;

		game.Map.Items.OnDestroyItem(this);
	}

	protected override void OnCollisionEffect(int pDamage)
	{
		if(!isSpawned)
			return;

		if(weaponConfig != null)
		{
			Debug.Log("todo: weapon explode");
		}
		if(pDamage <= 0)
			return;

		ReturnToPool();
	}

	//protected override bool CanSendMsg(EPhotonMsg pMsgType)
	//{
	//	switch(pMsgType)
	//	{
	//		//master initiates
	//		case EPhotonMsg.MapItem_InitMapSpecial:
	//		case EPhotonMsg.MapItem_InitMapBasic:
	//		case EPhotonMsg.MapItem_InitPowerUp:
	//			return view.IsMine;

	//		//only master can return items to pool (master owns all map items)
	//		case EPhotonMsg.MapItem_ReturnToPool:
	//			return !view.IsMine;
	//	}

	//	return false;
	//}

	//protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	//{
	//	base.HandleMsg(pReceivedMsg, pParams, bb);
	//	switch(pReceivedMsg)
	//	{
	//		case EPhotonMsg.MapItem_InitMapBasic:
	//			Vector3 pos = (Vector3)pParams[0];
	//			EWeaponId id = (EWeaponId)pParams[1];

	//			Init(pos, brainiacs.ItemManager.GetMapWeaponConfig(id));
	//			break;

	//		case EPhotonMsg.MapItem_InitMapSpecial:
	//			pos = (Vector3)pParams[0];
	//			id = (EWeaponId)pParams[1];

	//			Init(pos, brainiacs.ItemManager.GetMapSpecialWeaponConfig(id));
	//			break;

	//		case EPhotonMsg.MapItem_InitPowerUp:
	//			pos = (Vector3)pParams[0];
	//			EPowerUp type = (EPowerUp)pParams[1];

	//			Init(pos, brainiacs.ItemManager.GetPowerupConfig(type));
	//			break;

	//		case EPhotonMsg.MapItem_ReturnToPool:
	//			ReturnToPool();
	//			break;
	//	}
	//}

}
