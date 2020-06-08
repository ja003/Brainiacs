using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class DebugData
{
	public static bool TestRemote = false;

	public static bool TestMP = false;

	public static bool TestPlayers = false;
	public static bool LocalImage = false;

	private static int playerCount = 1;

	public static bool TestResult = false;

	public static bool TestMobileInput = false;

	public static bool TestNonAggressiveAi = false;
	public static bool TestAiDebugMove = false;

	public static bool TestGenerateItems = false;
	public static bool StopGenerateItems = false;
	public static EWeaponId TestGenerateMapWeapon = EWeaponId.None;
	public static EWeaponId TestGenerateMapSpecialWeapon = EWeaponId.None;

	public static EPowerUp TestPowerUp = EPowerUp.None;
	public static EPlayerEffect TestPlayerEffect = EPlayerEffect.None;

	public static bool TestExtraPlayerItem = false;

	public static bool TestShield = false;
	public static bool TestImmortality = false;


	public static EHero TestHero = EHero.Einstein;

	public static int TestGameValue = 10;




	public static EWeaponId TestAiWeapon = EWeaponId.None;

	public static void TestSetGameInitInfo()
	{
		if(playerCount >= 1)
		{
			PlayerInitInfo player1 = GetPlayerInitInfo(1);
			Brainiacs.Instance.GameInitInfo.AddPlayer(player1);
		}

		if(playerCount >= 2)
		{
			PlayerInitInfo player2 = GetPlayerInitInfo(2);
			Brainiacs.Instance.GameInitInfo.AddPlayer(player2);
		}

		//PlayerInitInfo player3 = DebugData.GetPlayerInitInfo(3);
		//GameInitInfo.AddPlayer(player3);

		Brainiacs.Instance.GameInitInfo.Mode = EGameMode.Score;
		Brainiacs.Instance.GameInitInfo.Map = EMap.Steampunk;
		Brainiacs.Instance.GameInitInfo.GameModeValue = 15;
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
					EPlayerColor.Pink, EPlayerType.AI);

				break;
			case 3:
				player = new PlayerInitInfo(pPlayerNumber,
			EHero.Currie, GetPlayerName(pPlayerNumber),
			EPlayerColor.Yellow, EPlayerType.LocalPlayer);
				break;
		}
		player.PhotonPlayer = PhotonNetwork.LocalPlayer;

		player.debug_StartupWeapon.Add(EWeaponId.Lasergun);
		player.debug_StartupWeapon.Add(EWeaponId.MP40);
		player.debug_StartupWeapon.Add(EWeaponId.Flamethrower);
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

