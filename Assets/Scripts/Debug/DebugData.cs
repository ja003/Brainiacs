﻿using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class DebugData
{
	private static bool release = false;

	public static bool TestRemote = false && !release;
	public static bool TestMP = false && !release;
	public static bool TestPlayers = true && !release;
	public static bool LocalImage = false && !release;
	public static bool TestResult = false && !release;
	public static bool TestPlatformMobile = false && !release;
	public static bool TestMobileJoystick = false;

	//Player
	public static EHero TestHero = release ? EHero.None : EHero.Tesla;
	public static bool TestExtraPlayerItem = false && !release;

	public static bool TestShield = false && !release;
	public static bool TestInvulnerability = false && !release; //cant receive any damage
	public static bool TestImmortality = false && !release; //cant get under < 1 health
	public static bool TestInfiniteAmmo = false && !release;
	public static EPlayerEffect TestPlayerEffect = release ? EPlayerEffect.None : EPlayerEffect.None;

	//GAME
	private static int playerCount = 3;
	public static EMap TestMap = release ?  EMap.None : EMap.Wonderland;
	public static int TestGameValue = 1;
	public static EPowerUp TestPowerUp = release ? EPowerUp.None : EPowerUp.Speed;
	public static bool TestGenerateItems = true && !release;
	public static bool StopGenerateItems = false && !release;
	public static EWeaponId TestGenerateMapWeapon = release ? EWeaponId.None : EWeaponId.None;
	public static EWeaponId TestGenerateMapSpecialWeapon = release ? EWeaponId.None : EWeaponId.None;

	//SOUND
	public static bool MuteMusic = false && !release;


	// AI
	public static EWeaponId TestAiWeapon = release ? EWeaponId.None : EWeaponId.None;
	public static bool TestNonAggressiveAi = false && !release;
	public static bool TestAiDebugMove = false && !release;

	public static void TestSetGameInitInfo()
	{
		for(int i = 1; i <= playerCount; i++)
		{
			PlayerInitInfo player = GetPlayerInitInfo(i);
			Brainiacs.Instance.GameInitInfo.AddPlayer(player);
		}

		//PlayerInitInfo player3 = DebugData.GetPlayerInitInfo(3);
		//GameInitInfo.AddPlayer(player3);

		Brainiacs.Instance.GameInitInfo.Mode = EGameMode.Deathmatch;
		Brainiacs.Instance.GameInitInfo.Map = TestMap == EMap.None ? EMap.Steampunk : TestMap;
		Brainiacs.Instance.GameInitInfo.GameModeValue = TestGameValue;
	}

	public static PlayerInitInfo GetPlayerInitInfo(int pPlayerNumber)
	{
		PlayerInitInfo player = null;
		switch(pPlayerNumber)
		{
			case 1:
				player = new PlayerInitInfo(pPlayerNumber,
					TestHero, GetPlayerName(pPlayerNumber),
					EPlayerColor.Green, EPlayerType.LocalPlayer);
				break;
			case 2:
				player = new PlayerInitInfo(pPlayerNumber,
					EHero.Nobel, GetPlayerName(pPlayerNumber),
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

		player.debug_StartupWeapon.Add(EWeaponId.Special_Einstein);
		player.debug_StartupWeapon.Add(EWeaponId.Special_DaVinci);
		player.debug_StartupWeapon.Add(EWeaponId.Special_Nobel);
		player.debug_StartupWeapon.Add(EWeaponId.Special_Tesla);
		//player.debug_StartupWeapon.Add(EWeaponId.Special_Curie);

		player.debug_StartupWeapon.Add(EWeaponId.Lasergun);
		player.debug_StartupWeapon.Add(EWeaponId.Flamethrower);
		player.debug_StartupWeapon.Add(EWeaponId.MP40);

		if(player.PlayerType == EPlayerType.AI && TestAiWeapon != EWeaponId.None)
		{
			player.debug_StartupWeapon.Add(TestAiWeapon);
		}

		return player;


	}

	internal static void TestSetResults()
	{
		Brainiacs.Instance.GameResultInfo = new GameResultInfo();

		PlayerScoreInfo result = PlayerScoreInfo.debug_PlayerResultInfo();
		if(playerCount >= 1)
		{
			result.Hero = TestHero;
			result.Name = $"test {result.Hero} player";
			result.Kills = 2;
			result.Deaths = 5;
			Brainiacs.Instance.GameResultInfo.PlayerResults.Add(result);
		}
		if(playerCount >= 2)
		{
			result = PlayerScoreInfo.debug_PlayerResultInfo();
			result.Hero = EHero.Tesla;
			result.Name = $"test {result.Hero} player";
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

	public static void OnBrainiacsAwake()
	{
		if(TestRemote || TestResult || TestMP || TestPlayers || LocalImage)
		{
			Debug.LogError("Testing data is ON - turn off when testing build");
		}
	}

	public static string GetPlayerName(int pIndex)
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




}

