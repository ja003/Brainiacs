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
	//public bool IsLocalRemote => DebugData.LocalRemote && LocalRemote == null;
	public bool IsLocalRemote;

	public bool IsItMe => InitInfo.IsItMe() && !IsLocalRemote;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			Health.DebugDie();
		}
	}
	
	public void SetInfo(PlayerInitInfo pPlayerInfo, bool pIsLocalRemote, Vector3? pSpawnPosition = null)
	{
		//Debug.Log($"{this} SetInfo");
		IsLocalRemote = pIsLocalRemote;
		//Debug.Log($"{this} SetInfo");
		InitInfo = pPlayerInfo;
		Visual.Init(pPlayerInfo);

		if(!pIsLocalRemote)
			Init();

		if(pSpawnPosition != null)
			Movement.SpawnAt((Vector3)pSpawnPosition);

		Network.Init(pPlayerInfo);
	}

	public void OnReceivedInitInfo(PlayerInitInfo pInfo)
	{
		//Debug.Log($"{this} OnReceivedInitInfo");
		SetInfo(pInfo, false);
		game.PlayerManager.AddPlayer(this);
		//Init();
	}

	private void Init()
	{
		//Debug.Log($"{this} Init {IsItMe}");
		if(!IsItMe)
			return;

		Stats.Init();
		input.Init(InitInfo);


		ItemController.AddMapWeapon(EWeaponId.Lasergun);
		ItemController.AddMapWeapon(EWeaponId.Biogun);

		ItemController.AddHeroBasicWeapon(InitInfo.Hero);
		ItemController.AddHeroSpecialWeapon(InitInfo.Hero);

		ItemController.AddMapWeaponSpecial(EWeaponId.Flamethrower);
		ItemController.AddMapWeapon(EWeaponId.MP40);
		ItemController.AddHeroSpecialWeapon(EHero.Einstein);
		ItemController.AddHeroSpecialWeapon(EHero.DaVinci);
	}

	public override string ToString()
	{
		int number = InitInfo == null ? -1 : InitInfo.Number;
		return $"{number}_{gameObject.name} {Network}";
	}

}
