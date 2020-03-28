using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class Player : GameBehaviour
{
	[SerializeField]
	private PlayerInput input;

	[SerializeField]
	public PlayerWeaponController WeaponController;

	[SerializeField]
	public PlayerVisual Visual;

	[SerializeField]
	public PlayerMovement Movement;

	[SerializeField]
	public PlayerHealth Health;

	public BoxCollider2D Collider => boxCollider2D;

	[SerializeField]
	public PlayerItemController ItemController;

	[SerializeField]
	public PlayerStats Stats;

	[SerializeField]
	public PlayerNetworkController Network;

	public PlayerInitInfo InitInfo;

	//DEBUG
	public Player LocalRemote;
	public bool IsLocalRemote => DebugData.LocalRemote && LocalRemote == null;

	public bool IsItMe => InitInfo.IsItMe();

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			Health.DebugDie();
		}
	}
	public void OnReceivedInitInfo(PlayerInitInfo pInfo)
	{
		SetInfo(pInfo);
		game.PlayerManager.AddPlayer(this);
	}

	public void SetInfo(PlayerInitInfo pPlayerInfo, Vector3? pSpawnPosition = null)
	{
		InitInfo = pPlayerInfo;
		Network.Init(pPlayerInfo);		

		//Debug.Log($"{this} SetInfo");
		Stats.Init(pPlayerInfo);

		input.Init(pPlayerInfo);


		HeroSpecialWeaponConfig heroSpecialConfig =
			brainiacs.ItemManager.GetHeroSpecialWeaponConfig(pPlayerInfo.Hero);
		ItemController.AddHeroSpecialWeapon(pPlayerInfo.Hero);


		ItemController.AddMapWeapon(EWeaponId.MP40);
		ItemController.AddMapWeapon(EWeaponId.Lasergun);
		ItemController.AddMapWeapon(EWeaponId.Biogun);

		ItemController.AddHeroBasicWeapon(pPlayerInfo.Hero);

		Visual.Init(pPlayerInfo);

		if(pSpawnPosition != null)
			Movement.SpawnAt((Vector3)pSpawnPosition);
	}

	public override string ToString()
	{
		int number = InitInfo == null ? -1 : InitInfo.Number;
		return $"{number}_{gameObject.name} {Network}";
	}

}
