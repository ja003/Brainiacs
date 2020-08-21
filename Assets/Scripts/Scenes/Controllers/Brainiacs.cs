using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CSingletion("Singletons/P_Brainiacs", true)]
public class Brainiacs : CSingleton<Brainiacs>
{
	public static bool SelfInitGame = true;

	public GameInitInfo GameInitInfo;// = new GameInitInfo();
	public GameResultInfo GameResultInfo = new GameResultInfo();

	[SerializeField] public Scenes Scenes;
	[SerializeField] public ItemManager ItemManager;
	[SerializeField] public HeroManager HeroManager;
	[SerializeField] public MapManager MapManager;
	[SerializeField] public PlayerKeysManager PlayerKeysManager;
	[SerializeField] public PhotonManager PhotonManager;

	[SerializeField] public PlayerPrefsController PlayerPrefs;
	[SerializeField] public AudioManager AudioManager;

	[SerializeField] GameObject debugLogConsole = null;

	protected override void Awake()
	{

		GameInitInfo = new GameInitInfo();

		if(DebugData.TestPlayers)
			DebugData.TestSetGameInitInfo();


		debugLogConsole.SetActive(!Application.isEditor);

		DebugData.OnBrainiacsAwake();
		base.Awake();
	}



	public void SetGameResultInfo(List<PlayerScoreInfo> pResults, int pTimePassed)
	{
		GameResultInfo = new GameResultInfo
		{
			PlayerResults = pResults,
			PlayTime = pTimePassed
		};
	}

	public void SyncGameInitInfo()
	{
		var gameInfoBytes = GameInitInfo.Serialize();
		MainMenu.Instance.Photon.Send(EPhotonMsg.MainMenu_SyncGameInfo, gameInfoBytes);
	}

}
