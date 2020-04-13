using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using PhotonPlayer = Photon.Realtime.Player;

public class Player : GameBehaviour
{
	[SerializeField] private PlayerInput input = null;

	[SerializeField] public PlayerWeaponController WeaponController;
	[SerializeField] public PlayerVisual Visual;
	[SerializeField] public PlayerHealth Health;
	[SerializeField] public PlayerItemController ItemController;
	[SerializeField] public PlayerStats Stats;
	[SerializeField] public PlayerPhotonController Photon;
	[SerializeField] public PlayerMovement Movement;

	public BoxCollider2D Collider => boxCollider2D;
	public PlayerInitInfo InitInfo;
	public bool IsItMe => InitInfo.IsItMe() && !IsLocalImage;
	public bool IsInited;

	//DEBUG
	[NonSerialized] public Player LocalImage;
	[NonSerialized] public Player LocalImageOwner;
	[NonSerialized] public bool IsLocalImage;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			Health.DebugDie();
		}
	}

	public void SetInfo(PlayerInitInfo pPlayerInfo, bool pIsLocalImage, Vector3? pSpawnPosition = null)
	{
		//Debug.Log($"{this} SetInfo");

		IsLocalImage = pIsLocalImage;
		InitInfo = pPlayerInfo;
		Visual.Init(pPlayerInfo);

		Init();

		if(pSpawnPosition != null)
			Movement.SpawnAt((Vector3)pSpawnPosition);

		Photon.Init(pPlayerInfo);
	}

	/// <summary>
	/// Called only on clients and local image
	/// </summary>
	public void OnReceivedInitInfo(PlayerInitInfo pInfo, bool pIsLocalImage)
	{
		//Debug.Log($"{this} OnReceivedInitInfo");
		SetInfo(pInfo, pIsLocalImage);
		if(!pIsLocalImage)
			game.PlayerManager.AddPlayer(this);
		//Init(); 
		IsInited = true;
		//Debug.Log("X_Inited_OnReceivedInitInfo");
	}


	/// <summary>
	/// This init is called only for local player
	/// </summary>
	private void Init()
	{
		//Debug.Log($"{this} Init {IsItMe}");
		if(!IsItMe) //player image controllers dont need initializing
			return;

		Stats.Init();
		input.Init(InitInfo);


		//NOTE: first added weapon will be active (PlayerWeaponController::SetDefaultWeaponActive)
		ItemController.AddHeroSpecialWeapon(EHero.Currie);
		ItemController.AddMapWeapon(EWeaponId.Lasergun);

		ItemController.AddMapWeapon(EWeaponId.Biogun);

		ItemController.AddHeroBasicWeapon(InitInfo.Hero);
		ItemController.AddHeroSpecialWeapon(InitInfo.Hero);
		ItemController.AddMapWeapon(EWeaponId.MP40);

		ItemController.AddHeroSpecialWeapon(EHero.DaVinci);
		ItemController.AddHeroSpecialWeapon(EHero.Einstein);
		ItemController.AddMapWeaponSpecial(EWeaponId.Flamethrower);
		IsInited = true;
		//Debug.Log("X_Inited_Init");

		game.PlayerManager.OnAllPlayersAdded.AddAction(OnAllPlayersAdded);

	}

	internal void OnAllPlayersAdded()
	{
		if(!IsItMe)
			return;
		WeaponController.SetDefaultWeaponActive();
	}

	public override string ToString()
	{
		int number = InitInfo == null ? -1 : InitInfo.Number;
		return $"{number}_{gameObject.name} {Photon}";
	}


}
