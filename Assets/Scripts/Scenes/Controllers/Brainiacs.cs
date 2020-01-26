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
		GameInitInfo.players.Add(new PlayerInitInfo(EHero.Tesla, "Adam"));
		GameInitInfo.players.Add(new PlayerInitInfo(EHero.Currie, "Téra"));

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
