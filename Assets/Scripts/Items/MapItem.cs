using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MapObject
{
	MapWeaponConfig weaponConfig;
	MapSpecialWeaponConfig weaponSpecialConfig;
	PowerUpConfig powerUpConfig;

	[SerializeField] MapItemPhoton photon;

	bool isSpawned;
	bool isMine;
	private void Spawn(Vector3 pPosition)
	{
		gameObject.SetActive(true);
		transform.position = pPosition;
		Debug.Log($"Item spawned at {pPosition}");

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

		photon.Send(EPhotonMsg.MapItem_InitPowerUp, pPosition, pPowerUp.Type);
	}

	public void Init(Vector3 pPosition, MapWeaponConfig pWeapon)
	{
		weaponConfig = pWeapon;
		spriteRend.sprite = pWeapon.MapItemInfo.MapSprite;
		Spawn(pPosition);

		photon.Send(EPhotonMsg.MapItem_InitMapBasic, pPosition, pWeapon.Id);

	}

	public void Init(Vector3 pPosition, MapSpecialWeaponConfig pSpecialWeapon)
	{
		weaponSpecialConfig = pSpecialWeapon;
		spriteRend.sprite = pSpecialWeapon.MapItemInfo.MapSprite;
		Spawn(pPosition);

		photon.Send(EPhotonMsg.MapItem_InitMapSpecial, pPosition, pSpecialWeapon.Id);
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
			Debug.Log("OnEnter powerup");
			PowerupManager.HandlePowerup(powerUpConfig, pPlayer);
		}
		else if(weaponConfig != null)
		{
			Debug.Log("OnEnter weapon");
			pPlayer.ItemController.AddMapWeapon(weaponConfig.Id);
		}
		else if(weaponSpecialConfig != null)
		{
			Debug.Log("OnEnter weapon special");
			pPlayer.ItemController.AddMapWeaponSpecial(weaponSpecialConfig.Id);
		}
		//TODO: special weapon + handle error

		ReturnToPool();
	}

	public void ReturnToPool()
	{
		if(isMine)
			InstanceFactory.Destroy(gameObject);
		else
			photon.Send(EPhotonMsg.MapItem_ReturnToPool);
	}

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
}
