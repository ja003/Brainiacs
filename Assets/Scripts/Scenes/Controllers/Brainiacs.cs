using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CSingletion("Singletons/P_Brainiacs", true)]
public class Brainiacs : CSingleton<Brainiacs>
{
	[SerializeField]
	public Scenes Scenes;

	public static bool SelfInitGame = true;

	public GameInitInfo GameInitInfo;// = new GameInitInfo();
	public GameResultInfo GameResultInfo;

	[SerializeField]
	public ItemManager ItemManager;

	[SerializeField]
	public HeroManager HeroManager;

	[SerializeField]
	public MapManager MapManager;

	[SerializeField]
	public PlayerKeysManager PlayerKeysManager;

	[SerializeField]
	public PhotonManager PhotonManager;

	[SerializeField] GameObject debugLogConsole;

	protected override void Awake()
	{
		base.Awake();

		GameInitInfo = new GameInitInfo();
		
		//TestSetGameInitInfo();


		debugLogConsole.SetActive(!Application.isEditor);

		DebugData.OnBrainiacsAwake();
	}

	public void TestSetGameInitInfo()
	{
		Debug.LogError("TestSetGameInitInfo");
		PlayerInitInfo player1 = DebugData.GetPlayerInitInfo(1);
		GameInitInfo.AddPlayer(player1);


		PlayerInitInfo player2 = DebugData.GetPlayerInitInfo(2);
		GameInitInfo.AddPlayer(player2);

		PlayerInitInfo player3 = DebugData.GetPlayerInitInfo(3);
		GameInitInfo.AddPlayer(player3);

		GameInitInfo.Mode = EGameMode.Time;
		GameInitInfo.Map = EMap.Steampunk;
		GameInitInfo.GameModeValue = 5;
	}

	public void SetGameResultInfo(List<Player> pPlayers)
	{
		GameResultInfo = new GameResultInfo();
		foreach(Player player in pPlayers)
		{
			GameResultInfo.PlayerResults.Add(new PlayerResultInfo(player.Stats));
		}

	}

	public void SyncGameInitInfo()
	{
		var gameInfoBytes = GameInitInfo.Serialize();
		MainMenu.Instance.Photon.Send(EPhotonMsg_MainMenu.SyncGameInfo, gameInfoBytes);
	}

}
