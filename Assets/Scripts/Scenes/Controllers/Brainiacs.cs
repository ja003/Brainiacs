using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CSingletion("Singletons/P_Brainiacs", true)]
public class Brainiacs : CSingleton<Brainiacs>
{
	[SerializeField]
	public Scenes Scenes;

	public static bool SelfInitGame = true;

	public GameInitInfo GameInitInfo;
	public GameResultInfo GameResultInfo;

	protected override void Awake()
	{
		base.Awake();
		TestSetGameInitInfo();
	}

	public void TestSetGameInitInfo()
	{
		GameInitInfo = new GameInitInfo();
		PlayerInitInfo player1 = new PlayerInitInfo(EHero.Tesla, "Adam");
		player1.PlayerKeys = new PlayerKeys(
			KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow,
			KeyCode.RightControl, KeyCode.RightShift);
		GameInitInfo.players.Add(player1);


		PlayerInitInfo player2 = new PlayerInitInfo(EHero.Currie, "Téra");

		player2.PlayerKeys = new PlayerKeys(
			KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A,
			KeyCode.LeftControl, KeyCode.LeftShift);
		GameInitInfo.players.Add(player2);

		GameInitInfo.Mode = EGameMode.Time;
		GameInitInfo.Map = EMap.Steampunk;
		GameInitInfo.Time = 5;
	}

	public void SetGameResultInfo(List<Player> pPlayers)
	{
		GameResultInfo = new GameResultInfo();
		foreach(Player player in pPlayers)
		{
			GameResultInfo.PlayerResults.Add(new PlayerResultInfo(player));
		}

	}

}
