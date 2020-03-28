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
		
		if(DebugData.TestPlayers)
			DebugData.TestSetGameInitInfo();


		debugLogConsole.SetActive(!Application.isEditor);

		DebugData.OnBrainiacsAwake();
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
		MainMenu.Instance.Photon.Send(EPhotonMsg.MainMenu_SyncGameInfo, gameInfoBytes);
	}

}
