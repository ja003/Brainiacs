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

	[SerializeField] public GamePhoton Photon;

	[SerializeField] public GameEndController GameEnd;

	[SerializeField] public UICurtain uiCurtain;

	[SerializeField] public GameTimeController GameTime;
	[SerializeField] public UIGameTime UIGameTime;

	[SerializeField] public PlayersResultManager Results;

	protected override void Awake()
	{
		brainiacs.SetOnAwaken(OnAwaken);

		Activate();

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

	[SerializeField] bool debug_forceCurtain;

	public new void Activate()
	{
		//Debug.Log("Game Activate");
		mainCamera.enabled = true;

		//if(Brainiacs.SelfInitGame && !debug_forceCurtain)
		if(Time.time < 1 && !debug_forceCurtain) //debug: start from game scene
			StartGame();
		else
			uiCurtain.SetFade(false, StartGame);

		base.Activate();
	}

	public bool GameStarted { get; private set; }
	private void StartGame()
	{
		//Debug.Log("StartGame");
		GameStarted = true;
	}
}
