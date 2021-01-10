using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;
using PhotonPlayer = Photon.Realtime.Player;

public class Player : PoolObjectNetwork, IOwner
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
	[SerializeField] public PlayerPush Push;

	[SerializeField] public AudioSource AudioSource;

	public PlayerInitInfo InitInfo;

	//should be called only after player is inited
	public bool IsItMe => InitInfo.IsItMe() && !IsLocalImage;
	public bool IsInited; //initialized only once (see OnReturnToPool2)

	//should be called only on update checks, not during an initializing method.
	public bool IsInitedAndMe => IsInited && IsItMe;



	//DEBUG
	[NonSerialized] public Player LocalImage;
	[NonSerialized] public Player LocalImageOwner;
	[NonSerialized] public bool IsLocalImage;
	[SerializeField] public bool debug_DrawPath;


	public Vector2 Position => Collider.bounds.center;
	public Vector3 Position3D => Collider.bounds.center;

	public BoxCollider2D Collider => boxCollider2D;
	//this is just "estimated size", not actual collider size (which is rectangle)
	public const float COLLIDER_SIZE = 0.5f;
	//actual collider size - just static getter (used in PathFinder)
	//public static Vector2 ColliderSize;

	protected override void Awake()
	{
		//ColliderSize = boxCollider2D.size;
		base.Awake();
	}

	protected override void OnSetActive0(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue;
		Visual.OnSetActive(pValue);

		if(pValue)
			game.PlayerManager.playerSorter.RegisterPlayer(this);
		else
			game.PlayerManager.playerSorter.UnregisterPlayer(this);
	}


	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			Health.DebugDie();
		}
	}

	public void SetInfo(PlayerInitInfo pPlayerInfo, bool pIsLocalImage, Vector2? pSpawnPosition = null)
	{
		//Debug.Log($"{this} SetInfo");

		//if already set as local image, keep it
		IsLocalImage = pIsLocalImage || IsLocalImage;
		//if(IsLocalImage)
		//{
		//	rigidBody2D.bodyType = RigidbodyType2D.Static;
		//	rigidBody2D.simulated = false;
		//}
		string suffix_LI = IsLocalImage ? "_LI" : "";
		string suffix_type = "_" + pPlayerInfo.PlayerType;
		string suffix_clone = ai.IsTmp ? "_clone" : "";
		gameObject.name = "Player_" + pPlayerInfo.Name + suffix_type + suffix_LI + suffix_clone;


		InitInfo = pPlayerInfo;
		Visual.Init(pPlayerInfo);

		Init();

		//if spawn pos is null, player is already spawned, but Movement should be still initialized
		Vector2 spawnPosition = pSpawnPosition != null ? (Vector2)pSpawnPosition : Position;
		Movement.SpawnAt(spawnPosition);

		((PlayerPhotonController)Photon).Init2(pPlayerInfo);
	}

	/// <summary>
	/// Called only on clients and local image
	/// </summary>
	public void OnReceivedInitInfo(PlayerInitInfo pInfo, bool pIsLocalImage)
	{
		Debug.Log($"{this} OnReceivedInitInfo");

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
	public bool CollidesWith(Vector2 pPosition)
	{
		return boxCollider2D.OverlapPoint(pPosition);
	}

	public float GetDistance(Vector2 pPosition)
	{
		return Vector2.Distance(transform.position, pPosition);
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

		if(IsInited)
			return;

		Stats.Init();
		input.Init(InitInfo);
		Health.Init();

		ItemController.Init(InitInfo.Hero);

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
			if(InitInfo == null || p.InitInfo == null)
				return false;
			return InitInfo.Number == p.InitInfo.Number &&
				InitInfo.PlayerType == p.InitInfo.PlayerType &&
				ai.IsTmp == p.ai.IsTmp; //for tesla clone
		}
	}

	public override int GetHashCode()
	{
		return InitInfo.Number;
	}

	protected override void OnReturnToPool2()
	{
		//todo: maybe it is ok to init player just once and not again
		//when reinstanced (eg. Tesla clone). All event listeners would have to be 
		//unregistered. For now only visual changes (in SetInfo)
		//IsInited = false;

		Stats.OnReturnToPool();
		game.PlayerManager.playerSorter.UnregisterPlayer(this);
	}

	public Player GetOwner()
	{
		return this;
	}
}
