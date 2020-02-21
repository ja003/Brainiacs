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

	[SerializeField]
	public ItemManager ItemManager;

	[SerializeField]
	public HeroManager HeroManager;

	protected override void Awake()
	{
		base.Awake();
		TestSetGameInitInfo();
	}

	public void TestSetGameInitInfo()
	{
		GameInitInfo = new GameInitInfo();

		PlayerInitInfo player1 = DebugData.GetPlayerInitInfo(1);		
		GameInitInfo.players.Add(player1);


		PlayerInitInfo player2 = DebugData.GetPlayerInitInfo(2);
		GameInitInfo.players.Add(player2);

		PlayerInitInfo player3 = DebugData.GetPlayerInitInfo(3);
		GameInitInfo.players.Add(player3);

		GameInitInfo.Mode = EGameMode.Time;
		GameInitInfo.Map = EMap.Steampunk;
		GameInitInfo.Time = 5;
	}

	public void SetGameResultInfo(List<Player> pPlayers)
	{
		GameResultInfo = new GameResultInfo();
		foreach(Player player in pPlayers)
		{
			GameResultInfo.PlayerResults.Add(new PlayerResultInfo(player.Stats));
		}

	}

}
