using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : GameBehaviour
{
	[SerializeField]
	private Button btnStartGame;

	protected/* override*/ void Awake()
	{
		//base.Awake();

		Brainiacs.InstantiateSingleton();
		btnStartGame.onClick.AddListener(OnBtnStartGame);
	}


	private void OnBtnStartGame()
	{
		Brainiacs.Instance.Scenes.LoadScene(EScene.Loading);
	}

}
