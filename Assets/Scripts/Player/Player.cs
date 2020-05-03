using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;
using PhotonPlayer = Photon.Realtime.Player;

public class Player : PoolObject
{
	[Header("Player")]
	[SerializeField] private PlayerInput input = null;

	[SerializeField] public PlayerWeaponController WeaponController;
	[SerializeField] public PlayerVisual Visual;
	[SerializeField] public PlayerHealth Health;
	[SerializeField] public PlayerItemController ItemController;
	[SerializeField] public PlayerStats Stats;
	//[SerializeField] public PlayerPhotonController Photon;
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

	protected override void OnSetActive(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue;
		Visual.OnSetActive(pValue);
	}

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

		((PlayerPhotonController)Photon).Init2(pPlayerInfo);
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

		SetActive(true);
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

	public override int GetHashCode()
	{
		return InitInfo.Number;
	}

	//PHOTON
	//bool photonInited;
	//public PhotonPlayer PhotonPlayer;

	//internal void InitPhoton(PlayerInitInfo pInfo)
	//{
	//	if(photonInited)
	//		return;
	//	PhotonPlayer = pInfo.PhotonPlayer;

	//	//Debug.Log($"{this} IsMine: {view.IsMine}, isItMe: {isItMe}");

	//	if(PhotonNetwork.IsMasterClient && !IsItMe && PhotonPlayer != null)
	//	{
	//		view.TransferOwnership(PhotonPlayer);
	//		Debug.Log("Transfer ownership to " + PhotonPlayer.NickName);
	//	}

	//	//Debug.Log(this + " send init Info");
	//	//DoInTime(() => Send(
	//	//	EPhotonMsg.Player_InitPlayer, pPlayerInfo.Number), 1);
	//	Photon.Send(EPhotonMsg.Player_InitPlayer, pInfo.Number);

	//	photonInited = true;
	//}

	/// <summary>
	///// Player can send data only if it is mine player and is inited.
	///// There are execeptions.
	///// </summary>
	//protected override bool CanSendMsg(EPhotonMsg pMsgType)
	//{
	//	switch(pMsgType)
	//	{
	//		case EPhotonMsg.Player_InitPlayer:
	//		case EPhotonMsg.Player_ShowWeapon:
	//			return true;
	//		case EPhotonMsg.Player_AddKill:
	//		case EPhotonMsg.Player_ApplyDamage:
	//			return IsInited;
	//	}

	//	return IsInitedAndMe;
	//}

	//protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	//{
	//	base.HandleMsg(pReceivedMsg, pParams, bb);

	//	if(!IsInited && pReceivedMsg != EPhotonMsg.Player_InitPlayer)
	//	{
	//		//Debug.Log("not inited yet. " + pReceivedMsg);
	//		return;
	//	}

	//	switch(pReceivedMsg)
	//	{
	//		case EPhotonMsg.Player_ChangeDirection:
	//			EDirection dir = (EDirection)pParams[0];
	//			Visual.OnDirectionChange(dir);
	//			break;

	//		case EPhotonMsg.Player_InitPlayer:
	//			int playerNumber = (int)pParams[0];
	//			PlayerInitInfo info = brainiacs.GameInitInfo.GetPlayer(playerNumber);
	//			OnReceivedInitInfo(info, false);
	//			break;

	//		case EPhotonMsg.Player_ShowWeapon:
	//			EWeaponId weapon = (EWeaponId)pParams[0];
	//			Visual.ShowWeapon(weapon);
	//			break;
	//		case EPhotonMsg.Player_ApplyDamage:
	//			int damage = (int)pParams[0];
	//			playerNumber = (int)pParams[1];
	//			Player originOfDamage = game.PlayerManager.GetPlayer(playerNumber);
	//			Health.ApplyDamage(damage, originOfDamage);
	//			break;

	//		case EPhotonMsg.Player_AddKill:
	//			//EPlayerStats stat = (EPlayerStats)pParams[0];
	//			//int value = (int)pParams[1];
	//			//bool force = (bool)pParams[2];
	//			bool force = (bool)pParams[0];
	//			Stats.AddKill(force);
	//			break;


	//		case EPhotonMsg.Player_UI_PlayerInfo_SetHealth:
	//			int health = (int)pParams[0];
	//			Visual.PlayerInfo.SetHealth(health);
	//			break;

	//		case EPhotonMsg.Player_UI_PlayerInfo_SetReloading:
	//			bool isReloading = (bool)pParams[0];
	//			float reloadTime = (float)pParams[1];
	//			Visual.PlayerInfo.SetReloading(isReloading, reloadTime);
	//			break;
	//		case EPhotonMsg.Player_UI_PlayerInfo_SetAmmo:
	//			int ammo = (int)pParams[0];
	//			Visual.PlayerInfo.SetAmmo(ammo);
	//			break;
	//		case EPhotonMsg.Player_UI_PlayerInfo_SetActiveWeapon:
	//			weapon = (EWeaponId)pParams[0];
	//			float cadency = (float)pParams[1];
	//			Visual.PlayerInfo.SetActiveWeapon(weapon, cadency);
	//			break;


	//		case EPhotonMsg.Player_UI_Scoreboard_SetScore:
	//			int kills = (int)pParams[0];
	//			int deaths = (int)pParams[1];
	//			Visual.Scoreboard.SetScore(kills, deaths);
	//			break;


	//		//default:
	//		//	Debug.LogError("Message not handled");
	//		//	break;
	//	}
	//}

	//protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	//{
	//	if(LocalImage)
	//	{
	//		LocalImage.HandleMsg(pMsgType, pParams);
	//	}
	//	else if(LocalImageOwner)
	//	{
	//		LocalImageOwner.HandleMsg(pMsgType, pParams);
	//	}
	//}
}
