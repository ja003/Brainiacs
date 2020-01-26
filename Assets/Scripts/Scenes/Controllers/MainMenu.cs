using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : GameBehaviour
{
	[SerializeField]
	private Button btnStartGame;

	public GameInitInfo GameInitInfo { get; private set; }

	protected/* override*/ void Awake()
	{
		//base.Awake();

		Brainiacs.InstantiateSingleton();
		btnStartGame.onClick.AddListener(OnBtnStartGame);
	}


	private void OnBtnStartGame()
	{
		brainiacs.TestSetGameInitInfo();
		Brainiacs.Instance.Scenes.LoadScene(EScene.Loading);
	}

	//private void TestSetGameInitInfo()
	//{
	//	GameInitInfo = new GameInitInfo();
	//	GameInitInfo.players.Add(new PlayerInitInfo(EHero.Tesla));
	//	GameInitInfo.players.Add(new PlayerInitInfo(EHero.Currie));

	//	GameInitInfo.Mode = EGameMode.Time;
	//	GameInitInfo.Time = 5;
	//}

}
