using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : CSingleton<Game>
{
	//[SerializeField] Camera mainCamera = null;

	[SerializeField] public MapController Map;
	[SerializeField] public PlayerManager PlayerManager;
	[SerializeField] public ProjectileManager ProjectileManager;
	[SerializeField] public UIPlayerStatusManager PlayerStatusManager;
	[SerializeField] public MobileInput MobileInput;
	[SerializeField] public GamePhoton Photon;
	[SerializeField] public GameEndController GameEnd;
	[SerializeField] public UICurtain uiCurtain;
	[SerializeField] public GameTimeController GameTime;
	[SerializeField] public UIGameTime UIGameTime;
	[SerializeField] public PlayersResultManager Results;
	[SerializeField] public PoolManager Pool;
	[SerializeField] public UIInfoMessengerGame InfoMessenger;
	[SerializeField] public LightingController Lighting;
	[SerializeField] public GameEffectManager GameEffect;

	[SerializeField] public GameAudioSource AudioSourcePrefab;


	[SerializeField] public Canvas UiCanvas;

	[SerializeField] public Layers Layers;

	[SerializeField] public GameDebug GameDebug;


	protected override void Awake()
	{
		//Debug.Log("Game Awake");
		//Debug.Log(brainiacs.GameInitInfo);

		brainiacs.SetOnAwaken(OnAwaken);

		//Activate();

		base.Awake(); //always call base.event() at the end
	}

	protected override void OnDestroy()
	{
		//UnityEngine.Debug.Log("Game OnDestroy");
	}

	private void OnAwaken()
	{
		//UnityEngine.Debug.Log("Game OnAwaken");

		//mainCamera?.enabled = false;
		//todo: deactivate players

		if(isMultiplayer && !PhotonNetwork.IsMasterClient)
		{
			Photon.Send(EPhotonMsg.Game_PlayerLoadedScene, PhotonNetwork.LocalPlayer.ActorNumber);
		}
		Activate();


		PlayerManager.OnAllPlayersAdded.AddAction(() => uiCurtain.SetFade(false, StartGame, debug_fastCurtain ? 0.1f : 1f));
	}

	[SerializeField] bool debug_fastCurtain = false; //debug: start from game scene

	public new void Activate()
	{
		//Debug.Log("Game Activate");
		//mainCamera.enabled = true;

		//if(Brainiacs.SelfInitGame && !debug_forceCurtain)
		//if(Time.time < 1 && !debug_forceCurtain) 
		//	StartGame();
		//else
		//	uiCurtain.SetFade(false, StartGame);

		//uiCurtain.SetFade(false, StartGame, debug_forceCurtain ? 1 : 0.1f);

		base.Activate();
	}

	public bool GameStarted { get; private set; }
	public Action OnGameStarted;
	private void StartGame()
	{
		//UnityEngine.Debug.Log("StartGame " + Time.time);
		GameStarted = true;
		InfoMessenger.Show("Game started!");
		OnGameStarted?.Invoke();
	}
}
