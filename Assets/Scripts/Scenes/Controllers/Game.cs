using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : CSingleton<Game>
{
	[SerializeField]
	Camera mainCamera;

	[SerializeField]
	public MapController MapController;

	[SerializeField]
	public PlayerManager PlayerManager;

	[SerializeField]
	public ProjectileManager ProjectileManager;

	[SerializeField]
	public UIPlayerStatusManager PlayerStatusManager;

	[SerializeField] public MobileInput MobileInput;

	[SerializeField] Button btnEnd;

	[SerializeField] GamePhoton Photon;

	protected override void Awake()
	{
		if(Brainiacs.SelfInitGame)
		{
			DoInTime(Activate, 0.5f);
		}
		btnEnd.onClick.AddListener(TestEndGame);

		OnAwaken();
		base.Awake(); //always call base.event() at the end
	}


	private void OnAwaken()
	{
		mainCamera.enabled = false;
		//todo: deactivate players

		if(brainiacs.GameInitInfo.IsMultiplayer() && !PhotonNetwork.IsMasterClient)
		{
			Photon.Send(EPhotonMsg.Game_PlayerLoadedScene, PhotonNetwork.LocalPlayer.ActorNumber);
		}
	}

	public new void Activate()
	{
		//Debug.Log("Game Activate");
		base.Activate();
		mainCamera.enabled = true;
	}

	public void TestEndGame()
	{
		brainiacs.SetGameResultInfo(PlayerManager.Players);
		brainiacs.Scenes.LoadScene(EScene.Results);
	}
}
