using Photon.Pun;
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

	[SerializeField] bool _MP;
	public bool MP => GetDebugBool(_MP);
	[SerializeField] bool _LocalImage;
	public bool LocalImage => GetDebugBool(_LocalImage);
	[SerializeField] bool _Result;
	public bool Result => GetDebugBool(_Result);
	[SerializeField] bool _PlatformMobile;
	public bool PlatformMobile => GetDebugBool(_PlatformMobile);

	[Header("Player")]
	[SerializeField] EHero _Hero;
	public EHero Hero => release ? EHero.None : _Hero;
	[SerializeField] bool _ExtraPlayerItem;
	public bool ExtraPlayerItem => GetDebugBool(_ExtraPlayerItem);

	[SerializeField] bool _Shield;
	public bool Shield => GetDebugBool(_Shield);
	[SerializeField] bool _Invulnerability;
	public bool Invulnerability => GetDebugBool(_Invulnerability); //cant receive any damage
	[SerializeField] bool _Immortality;
	public bool Immortality => GetDebugBool(_Immortality); //cant get under < 1 health
	[SerializeField] bool _InfiniteAmmo;
	public bool InfiniteAmmo => GetDebugBool(_InfiniteAmmo);
	[SerializeField] EPlayerEffect _PlayerEffect;
	public EPlayerEffect PlayerEffect => release ? EPlayerEffect.None : PlayerEffect;

	[Header("Game")]
	[SerializeField] int playerCount = 1;
	[SerializeField] EMap _Map;
	public EMap Map => release ? EMap.None : _Map;
	public int GameValue = 5;
	[SerializeField] bool _GenerateItems;
	public bool GenerateItems => GetDebugBool(_GenerateItems);
	[SerializeField] bool _StopGenerateItems;
	public bool StopGenerateItems => GetDebugBool(_StopGenerateItems);
	
	// generate items have priority: powerup, wepon, specialweapon
	[SerializeField] EGameEffect _GameEffect;
	public EGameEffect GameEffect => release ? EGameEffect.None : _GameEffect;
	
	[SerializeField] EPowerUp _PowerUp;
	public EPowerUp PowerUp => release ? EPowerUp.None : _PowerUp;
	
	[SerializeField] EWeaponId _GenerateMapWeapon;
	public EWeaponId GenerateMapWeapon => release ? EWeaponId.None : _GenerateMapWeapon;

	[SerializeField] EWeaponId _GenerateMapSpecialWeapon;
	public EWeaponId GenerateMapSpecialWeapon => release ? EWeaponId.None : _GenerateMapSpecialWeapon;

	[Header("Sound")]
	[SerializeField] bool _MuteMusic;
	public bool MuteMusic => GetDebugBool(_MuteMusic);

	// AI
	[SerializeField] EWeaponId _AiWeapon;
	public EWeaponId AiWeapon => release ? EWeaponId.None : _AiWeapon;
	[SerializeField] bool _NonAggressiveAi;
	public bool NonAggressiveAi => GetDebugBool(_NonAggressiveAi);
	[SerializeField] bool _AiDebugMove;
	public bool AiDebugMove => GetDebugBool(_AiDebugMove);



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
		return pValue && !release;
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
					Hero, GetPlayerName(pPlayerNumber),
					EPlayerColor.Green, EPlayerType.LocalPlayer,
					EKeyset.KeysetA);
				break;
			case 2:
				player = new PlayerInitInfo(pPlayerNumber,
					EHero.Currie, GetPlayerName(pPlayerNumber),
					EPlayerColor.Pink, EPlayerType.LocalPlayer);

				break;
			case 3:
				player = new PlayerInitInfo(pPlayerNumber,
					EHero.Currie, GetPlayerName(pPlayerNumber),
					EPlayerColor.Yellow, EPlayerType.LocalPlayer);
				break;

			default:
				player = new PlayerInitInfo(pPlayerNumber,
					EHero.Currie, GetPlayerName(pPlayerNumber),
					EPlayerColor.Blue, EPlayerType.LocalPlayer);
				break;
		}
		player.PhotonPlayer = PhotonNetwork.LocalPlayer;

		//player.debug_StartupWeapon.Add(EWeaponId.Special_Einstein);
		//player.debug_StartupWeapon.Add(EWeaponId.Special_Tesla);
		//player.debug_StartupWeapon.Add(EWeaponId.Special_Curie);
		//player.debug_StartupWeapon.Add(EWeaponId.Basic_Curie);
		//player.debug_StartupWeapon.Add(EWeaponId.Flamethrower);
		//player.debug_StartupWeapon.Add(EWeaponId.Special_Nobel);
		//player.debug_StartupWeapon.Add(EWeaponId.Special_Tesla);
		player.debug_StartupWeapon.Add(EWeaponId.MP40);
		//player.debug_StartupWeapon.Add(EWeaponId.Lasergun);
		//player.debug_StartupWeapon.Add(EWeaponId.Biogun);
		//player.debug_StartupWeapon.Add(EWeaponId.Mine);
		//player.debug_StartupWeapon.Add(EWeaponId.Special_DaVinci);

		if(player.PlayerType == EPlayerType.AI && AiWeapon != EWeaponId.None)
		{
			player.debug_StartupWeapon.Add(AiWeapon);
		}

		return player;


	}

	internal void SetResults()
	{
		Brainiacs.Instance.GameResultInfo = new GameResultInfo();

		PlayerScoreInfo result = PlayerScoreInfo.debug_PlayerResultInfo();
		if(playerCount >= 1)
		{
			result.Hero = Hero;
			result.Name = $" {result.Hero} player";
			result.Kills = 2;
			result.Deaths = 5;
			Brainiacs.Instance.GameResultInfo.PlayerResults.Add(result);
		}
		if(playerCount >= 2)
		{
			result = PlayerScoreInfo.debug_PlayerResultInfo();
			result.Hero = EHero.Tesla;
			result.Name = $" {result.Hero} player";
			result.Kills = 0;
			result.Deaths = 0;
			Brainiacs.Instance.GameResultInfo.PlayerResults.Add(result);
		}
		if(playerCount >= 3)
		{
			result = PlayerScoreInfo.debug_PlayerResultInfo();
			result.Name = "t Nobel";
			result.Hero = EHero.Nobel;
			result.Kills = 0;
			result.Deaths = 0;
			Brainiacs.Instance.GameResultInfo.PlayerResults.Add(result);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if(Result || MP || LocalImage)
		{
			Debug.LogError("testing data is ON - turn off in build");
		}
	}
	
	public string GetPlayerName(int pIndex)
	{
		switch(pIndex)
		{
			case 1:
				return "Adam";
			case 2:
				return "Téra";
			case 3:
				return "Johanka";
		}
		return "DEBUG_NAME";
	}

	






















	// INPUT DEBUG //

#if UNITY_EDITOR

	[SerializeField] float pushForce = 5;

	void Update()
	{
		Player player = Game.IsInstantiated ?
			Game.Instance.PlayerManager.GetPlayer(1) : null;

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

		else if(Input.GetKeyDown(KeyCode.L))
		{
			Game.Instance.PlayerManager.debug_OnPlayerLeftRoom();
		}

		else if(Input.GetKeyDown(KeyCode.M))
		{
			player.Health.ApplyDamage(50, null);
		}
		else if(Input.GetKeyDown(KeyCode.P))
		{
			player.Push.Push(Vector2.up * pushForce);
		}
		else if(Input.GetKeyDown(KeyCode.O))
		{
			player.Push.Push(Vector2.left * pushForce);
		}

		//HACK
		if(Input.GetKeyDown(KeyCode.Backslash))
		{
			player.Stats.StatsEffect.ApplyEffect(EPlayerEffect.DoubleSpeed, 1);
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
		else if(Input.GetKeyDown(KeyCode.S))
		{
			if(MainMenu.IsInstantiated)
				MainMenu.Instance.GameSetup.InfoMessenger.Show("Menu message");
		}
	}
#endif

	private void SetTimeScale(float pValue)
	{
		Time.timeScale = pValue;
		Debug.Log("SetTimeScale " + pValue);
	}

}