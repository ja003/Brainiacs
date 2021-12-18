﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//not practical - have to uncollapse value in inspector
//[Serializable]
//public class DebugBool
//{
//	[SerializeField] bool value;
//	public bool Get()
//	{
//		return value && !CDebug.Instance.release;
//	}
//}


[CSingletion("Singletons/P_CDebug", true)]
public class CDebug : CSingleton<CDebug>
{
	public bool release = false;
	public bool releaseWithExceptions = true;

	[SerializeField] bool _Result;
	public bool Result => GetDebugBool(_Result);
	[SerializeField] bool _PlatformMobile;
	public bool PlatformMobile => GetDebugBool(_PlatformMobile);

	[Header("Multiplayer")] //-----------------------------------------------//
	[SerializeField] bool _AutoJoinRandomRoom;
	public bool AutoJoinRandomGame => _AutoJoinRandomRoom;// GetDebugBool(_AutoJoinRandomRoom);
	[SerializeField] bool _LocalImage;
	public bool LocalImage => GetDebugBool(_LocalImage);

	[Header("Player")] //-----------------------------------------------//
	[SerializeField] int _debugPlayerNumber = 1;

	[SerializeField] EWeaponId _ExtraPlayerWeapon;
	public EWeaponId ExtraPlayerWeapon => release ? EWeaponId.None : _ExtraPlayerWeapon;

	[Tooltip("Works only on local player. see PlayerStats::IsShielded")]
	[SerializeField] bool _Shield;
	public bool Shield => GetDebugBool(_Shield);
	[SerializeField] bool _Invulnerability;
	public bool Invulnerability => GetDebugBool(_Invulnerability); //cant receive any damage

	[SerializeField] bool _Immortality;
	public bool Immortality => GetDebugBool(_Immortality); //cant get under < 1 health

	[SerializeField] bool _InstaKill;
	public bool InstaKill => GetDebugBool(_InstaKill); //every damage taken is MAX_HEALTH

	[SerializeField] bool _InfiniteAmmo;
	public bool InfiniteAmmo => GetDebugBool(_InfiniteAmmo);

	[SerializeField] bool _FastReaload;
	public bool FastReaload => GetDebugBool(_FastReaload);

	[SerializeField] EPlayerEffect _PlayerEffect;
	public EPlayerEffect PlayerEffect => release ? EPlayerEffect.None : _PlayerEffect;

	[Header("Game")] //-----------------------------------------------//
	[SerializeField] int playerCount = 1;
	[SerializeField] List<EPlayerType> playerTypes;
	[SerializeField] List<EHero> heroes;
	public EHero GetHero(int pPlayerNumber)
	{
		if(pPlayerNumber <= 0)
		{
			Debug.LogWarning("GetHero " + pPlayerNumber);
			return EHero.None;
		}
		if(heroes.Count < pPlayerNumber)
			return GetHero(pPlayerNumber - 1);

		return release ? EHero.None : heroes[pPlayerNumber - 1];
	}
	[SerializeField] EMap _Map;
	public EMap Map => release ? EMap.None : _Map;

	[SerializeField] int _gameValue;
	public int GameValue => release ? -1 : _gameValue;

	[SerializeField] bool _GenerateItems;
	public bool GenerateItems => GetDebugBool(_GenerateItems);
	[SerializeField] bool _Respawnpoint;
	public bool Respawnpoint => GetDebugBool(_Respawnpoint);
	[SerializeField] bool _StopGenerateItems;
	public bool StopGenerateItems => GetDebugBool(_StopGenerateItems);
	[SerializeField] bool _DontEndGame;
	public bool DontEndGame => GetDebugBool(_DontEndGame);

	// generate items have priority: powerup, weapon, specialweapon
	[SerializeField] EGameEffect _GameEffect;
	public EGameEffect GameEffect => release ? EGameEffect.None : _GameEffect;

	[SerializeField] EPowerUp _PowerUp;
	public EPowerUp PowerUp => release ? EPowerUp.None : _PowerUp;

	[SerializeField] EWeaponId _GenerateMapWeapon;
	public EWeaponId GenerateMapWeapon => release ? EWeaponId.None : _GenerateMapWeapon;

	[SerializeField] EWeaponId _GenerateMapSpecialWeapon;
	public EWeaponId GenerateMapSpecialWeapon => release ? EWeaponId.None : _GenerateMapSpecialWeapon;

	[Header("Sound")] //-----------------------------------------------//
	[SerializeField] bool _MuteMusic;
	public bool MuteMusic => GetDebugBool(_MuteMusic);

	// AI
	[Header("AI")] //-----------------------------------------------//
	[SerializeField] EWeaponId _AiWeapon;
	public EWeaponId AiWeapon => release ? EWeaponId.None : _AiWeapon;
	[SerializeField] bool _NonAggressiveAi;
	public bool NonAggressiveAi => GetDebugBool(_NonAggressiveAi);
	[SerializeField] bool _AiDebugMove;

	[SerializeField] bool _PassiveAi;
	public bool PassiveAi => GetDebugBool(_PassiveAi);

	public bool AiDebugMove => GetDebugBool(_AiDebugMove);

	[Header("Tutorial")] //-----------------------------------------------//
	[SerializeField] bool _SkipTutorial;
	public bool SkipTutorial => GetDebugBool(_SkipTutorial);
	[SerializeField] bool _ForceTutorial;
	public bool ForceTutorial => GetDebugBool(_ForceTutorial);

	[Header("Menu")] //-----------------------------------------------//
	[SerializeField] bool _InstaAnimation;
	public bool InstaAnimation => GetDebugBool(_InstaAnimation);

	[SerializeField] bool _OpenMenu;
	public bool OpenMenu => GetDebugBool(_OpenMenu);

	[SerializeField] bool _InstaReady;
	public bool InstaReady => GetDebugBool(_InstaReady);

	[SerializeField] EPlayerType _ExtraPlayerAtStart;
	public EPlayerType ExtraPlayerAtStart => release ? EPlayerType.None : _ExtraPlayerAtStart;

	[SerializeField] EPlayerType _ExtraPlayerAtStart2;
	public EPlayerType ExtraPlayerAtStart2 => release ? EPlayerType.None : _ExtraPlayerAtStart2;

	internal void Reset()
	{
		Debug.Log("RESET");
		var bools = this.GetType()
			  .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
			  .Where(x => x.FieldType == typeof(bool));
		foreach(var f in bools)
		{
			f.SetValue(this, false);
		}

		//todo: enum reset?

		release = true;
	}


	//stupid
	//private int GetDebugEnum(int pValue)
	//{
	//	return !release ? pValue : 0;
	//}


	private bool GetDebugBool(bool pValue)
	{
		return pValue && (!release || releaseWithExceptions);
	}


	public void SetGameInitInfo()
	{
		if(debug.release)
		{
			Debug.LogError("SetGameInitInfo called in release");
			return;
		}

		for(int i = 1; i <= playerCount; i++)
		{
			PlayerInitInfo player = GetPlayerInitInfo(i);
			Brainiacs.Instance.GameInitInfo.UpdatePlayer(player);
		}

		//PlayerInitInfo player3 = debug.GetPlayerInitInfo(3);
		//GameInitInfo.AddPlayer(player3);

		Brainiacs.Instance.GameInitInfo.Mode = EGameMode.Score;
		Brainiacs.Instance.GameInitInfo.Map = Map == EMap.None ? EMap.Steampunk : Map;
		Brainiacs.Instance.GameInitInfo.GameModeValue = GameValue;
	}

	public PlayerInitInfo GetPlayerInitInfo(int pPlayerNumber)
	{
		PlayerInitInfo player = null;
		switch(pPlayerNumber)
		{
			case 1:
				player = new PlayerInitInfo(pPlayerNumber,
					GetHero(1), PlayerNameManager.GetPlayerName(pPlayerNumber),
					EPlayerColor.Blue, playerTypes[0],
					EKeyset.A);
				break;
			case 2:
				player = new PlayerInitInfo(pPlayerNumber,
					GetHero(2), PlayerNameManager.GetPlayerName(pPlayerNumber),
					EPlayerColor.Pink, playerTypes[1]);

				break;
			case 3:
				player = new PlayerInitInfo(pPlayerNumber,
					GetHero(3), PlayerNameManager.GetPlayerName(pPlayerNumber),
					EPlayerColor.Yellow, playerTypes[2]);
				break;
			case 4:
				player = new PlayerInitInfo(pPlayerNumber,
					GetHero(4), PlayerNameManager.GetPlayerName(pPlayerNumber),
					EPlayerColor.Green, playerTypes[2]);
				break;

			default:
				player = new PlayerInitInfo(pPlayerNumber,
					EHero.Currie, PlayerNameManager.GetPlayerName(pPlayerNumber),
					EPlayerColor.Blue, EPlayerType.LocalPlayer);
				break;
		}



		return player;
	}

	/// <summary>
	/// Called during player items init
	/// </summary>
	internal void SetDebugStartupWeapon(ref PlayerInitInfo initInfo)
	{
		if(release && !releaseWithExceptions)
			return;

		//initInfo.debug_StartupWeapon.Add(EWeaponId.MP40);
		//initInfo.debug_StartupWeapon.Add(EWeaponId.Special_Curie);

		initInfo.debug_StartupWeapon.Add(ExtraPlayerWeapon);
		if(initInfo.PlayerType == EPlayerType.AI)
		{
			initInfo.debug_StartupWeapon.Add(AiWeapon);
		}
	}

	internal void SetResults()
	{
		Brainiacs.Instance.GameResultInfo = new GameResultInfo();

		PlayerScoreInfo result = PlayerScoreInfo.debug_PlayerResultInfo();
		if(playerCount >= 1)
		{
			result.Hero = GetHero(1);
			result.Name = $"{result.Hero} player";
			result.Color = EPlayerColor.Blue;
			result.Kills = 2;
			result.Deaths = 5;
			Brainiacs.Instance.GameResultInfo.PlayerResults.Add(result);
		}
		if(playerCount >= 2)
		{
			result = PlayerScoreInfo.debug_PlayerResultInfo();
			result.Hero = GetHero(2);
			result.Color = EPlayerColor.Red;
			result.Name = $"{result.Hero} player long name";
			result.Kills = 0;
			result.Deaths = 0;
			Brainiacs.Instance.GameResultInfo.PlayerResults.Add(result);
		}
		if(playerCount >= 3)
		{
			result = PlayerScoreInfo.debug_PlayerResultInfo();
			result.Name = "t Nobel";
			result.Color = EPlayerColor.Yellow;
			result.Hero = GetHero(3);
			result.Kills = 0;
			result.Deaths = 0;
			Brainiacs.Instance.GameResultInfo.PlayerResults.Add(result);
		}
		if(playerCount >= 4)
		{
			result = PlayerScoreInfo.debug_PlayerResultInfo();
			result.Name = "Mrs Curie";
			result.Color = EPlayerColor.Green;
			result.Hero = GetHero(4);
			result.Kills = 3;
			result.Deaths = 3;
			Brainiacs.Instance.GameResultInfo.PlayerResults.Add(result);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if(Result || AutoJoinRandomGame || LocalImage)
		{
			Debug.LogError("testing data is ON - turn off in build");
		}
	}
























	// INPUT DEBUG //

#if UNITY_EDITOR

	[SerializeField] float pushForce = 5;

	void Update()
	{
		Player player = Game.IsInstantiated ?
			Game.Instance.PlayerManager.GetPlayer(_debugPlayerNumber) : null;

		if(Input.GetKeyDown(KeyCode.F1))
		{
			SetTimeScale(0);
		}
		else if(Input.GetKeyDown(KeyCode.F2))
		{
			SetTimeScale(0.5f);
		}
		else if(Input.GetKeyDown(KeyCode.F3))
		{
			SetTimeScale(1);
		}
		else if(Input.GetKeyDown(KeyCode.F4))
		{
			SetTimeScale(2);
		}
		else if(Input.GetKeyDown(KeyCode.F5))
		{
			SetTimeScale(5);
		}

		//Player

		//else if(Input.GetKeyDown(KeyCode.L))
		//{
		//	Game.Instance.PlayerManager.debug_OnPlayerLeftRoom();
		//}

		//else if(Input.GetKeyDown(KeyCode.M))
		//{
		//	player.Health.ApplyDamage(50, null);
		//}
		//else if(Input.GetKeyDown(KeyCode.P))
		//{
		//	player.Push.Push(Vector2.up * pushForce);
		//}
		//else if(Input.GetKeyDown(KeyCode.O))
		//{
		//	player.Push.Push(Vector2.left * pushForce);
		//}

		//HACK
		if(Input.GetKeyDown(KeyCode.O))
		{
			player.Stats.StatsEffect.ApplyEffect(EPlayerEffect.DoubleSpeed, 10);
		}
		if(Input.GetKeyDown(KeyCode.P))
		{
			Game.Instance.Map.Items.generator.debug_GenerateItem();
			//player.Stats.StatsEffect.ApplyEffect(EPlayerEffect.HalfSpeed, 10);
		}



		else if(Input.GetKeyDown(KeyCode.N))
		{
			Game.Instance.Lighting.SetMode(!Game.Instance.Lighting.IsNight);
		}
		else if(Input.GetKeyDown(KeyCode.B))
		{
			Game.Instance.Lighting.SetNight(3);
		}


		//INFO MESSAGE
		else if(Input.GetKeyDown(KeyCode.U))
		{
			if(MainMenu.IsInstantiated)
				MainMenu.Instance.InfoMessenger.Show("Menu message");
			if(Game.IsInstantiated)
				Game.Instance.InfoMessenger.Show($"Game message. 1st player =  {Game.Instance.PlayerManager.Players[0].InitInfo.GetName(true)} !!!");
		}
	}
#endif

	private void SetTimeScale(float pValue)
	{
		Time.timeScale = pValue;
		Debug.Log("SetTimeScale " + pValue);
	}

}