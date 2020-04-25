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

	[SerializeField] public PlayerAiBrain ai;

	public BoxCollider2D Collider => boxCollider2D;
	public PlayerInitInfo InitInfo;

	//should be called only after player is inited
	public bool IsItMe => InitInfo.IsItMe() && !IsLocalImage;
	public bool IsInited;

	//should be called only on update checks, not during an initializing method.
	public bool IsInitedAndMe => IsInited && IsItMe;

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
		OnPlayerInited.Invoke();
		//Debug.Log("X_Inited_OnReceivedInitInfo");
	}

	/// <summary>
	/// True if pPosition is inside of players collider
	/// </summary>
	public bool CollidesWith(Vector3 pPosition)
	{
		return boxCollider2D.OverlapPoint(pPosition);
	}

	public float GetDistance(Vector3 pPosition)
	{
		return Vector3.Distance(transform.position, pPosition);
	}

	public ActionControl OnPlayerInited = new ActionControl();

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

		ItemController.Init(InitInfo.Hero);

		//ItemController.AddHeroBasicWeapon(InitInfo.Hero);

		//ItemController.AddHeroSpecialWeapon(EHero.Nobel);
		//ItemController.AddHeroSpecialWeapon(EHero.Currie);
		//ItemController.AddMapWeapon(EWeaponId.Lasergun);

		//ItemController.AddMapWeapon(EWeaponId.Biogun);

		//ItemController.AddHeroSpecialWeapon(InitInfo.Hero);
		//ItemController.AddMapWeapon(EWeaponId.MP40);

		//ItemController.AddHeroSpecialWeapon(EHero.DaVinci);
		//ItemController.AddHeroSpecialWeapon(EHero.Einstein);
		//ItemController.AddMapWeaponSpecial(EWeaponId.Flamethrower);

		if(InitInfo.PlayerType == EPlayerType.AI)
			ai.Init();

		IsInited = true;
		//Debug.Log("X_Inited_Init");

		game.PlayerManager.OnAllPlayersAdded.AddAction(OnAllPlayersAdded);
		OnPlayerInited.Invoke();
	}

	internal void OnAllPlayersAdded()
	{
		if(!IsItMe)
			return;

		//WeaponController.SetDefaultWeaponActive(); //handle in playerItemController
	}

	public override string ToString()
	{
		int number = InitInfo == null ? -1 : InitInfo.Number;
		return $"{number}_{gameObject.name} {Photon}";
	}

	public override bool Equals(object obj)
	{
		//Check for null and compare run-time types.
		if((obj == null) || !this.GetType().Equals(obj.GetType()))
		{
			return false;
		}
		else
		{
			Player p = (Player)obj;
			return InitInfo.Number == p.InitInfo.Number;
		}
	}
}
