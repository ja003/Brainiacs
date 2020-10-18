using CompanyNamespaceWhatever;
using FlatBuffers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using PhotonPlayer = Photon.Realtime.Player;


public class UIGameSetupMain : MainMenuController
{
	[Header("Players")]
	[SerializeField] GameObject btnGroupAddPlayer = null;
	[SerializeField] Button btnAddPlayerAi = null;
	[SerializeField] Button btnAddPlayerLocal = null;
	[SerializeField] Button btnAddPlayerRemote = null;
	[SerializeField] Transform playersHolder = null;
	[SerializeField] UIGameSetupPlayerEl playerPrefab = null;
	List<UIGameSetupPlayerEl> players = new List<UIGameSetupPlayerEl>();

	[Header("Game mode")]
	[SerializeField] Toggle gameModeToggleTime = null;
	[SerializeField] Toggle gameModeToggleScore = null;
	[SerializeField] Toggle gameModeToggleDeathmatch = null;
	[SerializeField] UiTextSwapper gameModeValueSwapper = null;

	[Header("Map")]
	[SerializeField] Image mapPreview = null;
	[SerializeField] UiTextSwapper mapSwapper = null;

	[Header("Buttons")]
	[SerializeField] Button btnBack = null;
	[SerializeField] Button btnPlay = null; //master
	[SerializeField] Button btnReady = null; //client
	[SerializeField] TextMeshProUGUI btnReadyText = null;
	[SerializeField] Button btnAllowJoin = null;

	[Header("DEBUG")]
	[SerializeField] Button debug_btnSyncInfo = null;

	GameInitInfo gameInitInfo => brainiacs.GameInitInfo;


	const int GAME_VALUE_MIN = 1;
	const int GAME_VALUE_MAX = 10;

	protected override void Awake()
	{
		btnBack.onClick.AddListener(mainMenu.GameSetup.OnSubMenuBtnBack);
		btnPlay.onClick.AddListener(OnBtnPlay);
		btnReady.onClick.AddListener(OnBtnReady);
		btnAllowJoin.onClick.AddListener(OnBtnAllowJoin);
		debug_btnSyncInfo.onClick.AddListener(OnBtnSyncInfo);
		base.Awake();
	}

	private void Update()
	{
		btnAllowJoin.interactable = PhotonNetwork.IsConnectedAndReady;
		btnReady.interactable = PhotonNetwork.IsConnectedAndReady;
		btnPlay.interactable = isMultiplayer ? PhotonNetwork.IsConnectedAndReady : true;
	}

	internal void SetGameInfo(GameInitInfo pGameInfo)
	{

		for(int i = 0; i < pGameInfo.Players.Count; i++)
		{
			PlayerInitInfo player = pGameInfo.Players[i];
			AddPlayer(player);
		}
		EGameMode mode = pGameInfo.Mode;
		gameModeToggleTime.isOn = mode == EGameMode.Time;
		gameModeToggleScore.isOn = mode == EGameMode.Score;
		gameModeToggleDeathmatch.isOn = mode == EGameMode.Deathmatch;

		mapSwapper.SetValue((int)pGameInfo.Map);

		gameModeValueSwapper.SetNumberValue(pGameInfo.GameModeValue);
	}

	bool isMaster;

	private void SetMenuMode(bool pIsMaster)
	{
		isMaster = pIsMaster;

		btnAllowJoin.gameObject.SetActive(pIsMaster);
		debug_btnSyncInfo.gameObject.SetActive(pIsMaster);
		btnGroupAddPlayer.SetActive(pIsMaster);

		gameModeToggleTime.interactable = pIsMaster;
		gameModeToggleScore.interactable = pIsMaster;
		gameModeToggleDeathmatch.interactable = pIsMaster;

		mapSwapper.SetInteractable(pIsMaster);
		gameModeValueSwapper.SetInteractable(pIsMaster);

		btnPlay.gameObject.SetActive(pIsMaster);
		btnReady.gameObject.SetActive(!pIsMaster);
	}

	public void SetActive(bool pValue, bool pIsMaster = false)
	{
		SetMenuMode(pIsMaster);
		isJoinAllowed = false;

		//reset elements
		foreach(var p in players)
		{
			p.gameObject.SetActive(false);
		}

		if(pValue)
		{
			if(pIsMaster)
			{
				AddPlayer(EPlayerType.LocalPlayer);

				//set default vlaues
				OnTimeToggled(true);
				OnScoreToggled(true);
				OnMapChanged();
				OnGameModeValueChanged();
			}
			//else
			//{
			//	for(int i = 0; i < gameInitInfo.Players.Count; i++)
			//	{
			//		PlayerInitInfo player = gameInitInfo.Players[i];
			//		AddPlayer(player);
			//	}
			//	EGameMode mode = brainiacs.GameInitInfo.Mode;
			//	gameModeToggleTime.isOn = mode == EGameMode.Time;
			//	gameModeToggleScore.isOn = mode == EGameMode.Score;
			//	gameModeToggleDeathmatch.isOn = mode == EGameMode.Deathmatch;

			//	mapSwapper.SetValue((int)brainiacs.GameInitInfo.Map);

			//	gameModeValueSwapper.SetNumberValue(
			//		brainiacs.GameInitInfo.GameModeValue);
			//}
		}
		base.SetActive(pValue);
	}

	/// <summary>
	/// Has to be called after configs are loaded (not after awake)
	/// </summary>
	protected override void OnMainControllerAwaken()
	{
		btnAddPlayerAi.onClick.
			AddListener(() => AddPlayer(EPlayerType.AI));
		btnAddPlayerLocal.onClick.
			AddListener(() => AddPlayer(EPlayerType.LocalPlayer));
		btnAddPlayerRemote.onClick.
			AddListener(() => AddPlayer(EPlayerType.RemotePlayer));


		playerPrefab.gameObject.SetActive(false);

		gameModeValueSwapper.InitNumberSwapper(GAME_VALUE_MIN, GAME_VALUE_MAX, OnGameModeValueChanged);
		gameModeToggleTime.onValueChanged.AddListener(OnTimeToggled);
		//gameModeToggleTime.isOn = true;
		if(DebugData.TestGameValue > 0)
			gameModeValueSwapper.SetNumberValue(DebugData.TestGameValue); //debug
		OnTimeToggled(true);

		gameModeToggleScore.onValueChanged.AddListener(OnScoreToggled);
		gameModeToggleDeathmatch.onValueChanged.AddListener(OnDeathmatchToggled);

		//DEBUG game mode
		//gameModeToggleScore.isOn = true;
		//OnGameModeToggled(true, EGameMode.Score);

		mapSwapper.Init(Utils.GetStrings(typeof(EMap)), OnMapChanged, 0);
	}

	/// MULTIPLAYER

	bool isJoinAllowed;
	private void OnBtnAllowJoin()
	{
		isJoinAllowed = true;
		//todo: check if any remote + check max players
		int playersCount = GetActivatedPlayers().Count;
		brainiacs.PhotonManager.CreateRoom(
			playersCount, OnRemotePlayerEnteredRoom);
		btnGroupAddPlayer.SetActive(false);
	}

	private void OnBtnSyncInfo()
	{
		Debug.LogError("DEBUG sync");
		brainiacs.SyncGameInitInfo();
	}

	private void OnRemotePlayerEnteredRoom(PhotonPlayer pPlayer)
	{
		Debug.Log("OnRemotePlayerEnteredRoom " + pPlayer);
		List<UIGameSetupPlayerEl> availableRemotes = GetAvailableRemotePlayers();
		if(availableRemotes.Count == 0)
		{
			Debug.LogError("No free remote element rdy. todo: kick out of room");
			return;
		}
		availableRemotes[0].OnRemoteConnected(pPlayer);

		brainiacs.SyncGameInitInfo();

		//Debug.LogError("todo: sync data to " + pPlayer);
		//data is auto synced in OnRemoteConnected
		//todo: send data just to this player?
	}


	private void OnMapChanged()
	{
		brainiacs.GameInitInfo.Map = (EMap)mapSwapper.CurrentIndex;
		mapPreview.sprite = brainiacs.MapManager.
			GetMapConfig(brainiacs.GameInitInfo.Map).MapPreview;
	}

	private void OnGameModeValueChanged()
	{
		gameInitInfo.GameModeValue = GAME_VALUE_MIN + gameModeValueSwapper.CurrentIndex;
		Debug.Log("gameModeValue = " + gameInitInfo.GameModeValue);
	}

	private void OnDeathmatchToggled(bool pValue)
	{
		OnGameModeToggled(pValue, EGameMode.Deathmatch);
	}

	private void OnScoreToggled(bool pValue)
	{
		Debug.Log("OnScoreToggled " + pValue);
		OnGameModeToggled(pValue, EGameMode.Score);
	}

	private void OnTimeToggled(bool pValue)
	{
		OnGameModeToggled(pValue, EGameMode.Time);
	}

	private void OnGameModeToggled(bool pValue, EGameMode pGameMode)
	{
		if(!pValue)
			return;
		gameInitInfo.Mode = pGameMode;
	}

	//remote
	private void AddPlayer(PlayerInitInfo pPlayer)
	{
		UIGameSetupPlayerEl addedElement = AddPlayerElement();
		addedElement.Init(pPlayer);
	}

	//MASTER
	private void AddPlayer(EPlayerType pType)
	{
		btnGroupAddPlayer.transform.parent = null;

		UIGameSetupPlayerEl addedElement = AddPlayerElement();
		addedElement.Init(players.IndexOf(addedElement) + 1, pType, null);


		//todo: check player count (max 4?)
		//keep btnAddPlayer at the end of the list
		btnGroupAddPlayer.transform.parent = playersHolder;

		OnPlayersChanged();
	}


	internal void UpdatePlayer(PlayerInitInfo pPlayerInfo)
	{
		GetPlayerEl(pPlayerInfo.Number).UpdateInfo(pPlayerInfo);
	}

	private UIGameSetupPlayerEl GetPlayerEl(int pNumber)
	{
		return GetActivatedPlayers().Find(a => a.Info.Number == pNumber);
	}

	private UIGameSetupPlayerEl AddPlayerElement()
	{
		UIGameSetupPlayerEl addedElement = null;
		bool reActivated = false;
		foreach(var p in players)
		{
			if(!p.gameObject.activeSelf)
			{
				addedElement = p;
				reActivated = true;
				//todo: reset element
				break;
			}
		}
		if(!reActivated)
		{
			addedElement = Instantiate(playerPrefab, playersHolder);
			players.Add(addedElement);
		}

		return addedElement;
	}


	public void OnPlayersChanged()
	{
		int activatedPlayersCount = GetActivatedPlayers().Count;
		bool isMaxPlayersCount = activatedPlayersCount == 4;
		bool canAddPlayer = !isJoinAllowed && !isMaxPlayersCount;
		//cant add any player if join is allowed or there already are max players
		btnGroupAddPlayer.SetActive(canAddPlayer);

		if(activatedPlayersCount > 4)
		{
			Debug.LogError("There are more than 4 players");
		}

		bool alreadyHasLocalPlayer =
			PlatformManager.IsMobile() && GetLocalPlayer() != null;

		btnAddPlayerLocal.gameObject.SetActive(!alreadyHasLocalPlayer);
	}

	private void OnBtnReady()
	{
		UIGameSetupPlayerEl myPLayerEl = GetMyPlayer();
		bool isReady = myPLayerEl.Info.IsReady;
		myPLayerEl.SetReady(!isReady);
		btnReadyText.text = myPLayerEl.Info.IsReady ? "NOT - READY" : "READY";
	}

	private void OnBtnPlay()
	{
		if(GetLocalPlayer() == null)
		{
			Debug.LogError("Cant start game with no local player");
			return;
		}

		if(gameInitInfo.Players.Count < 1)
		{
			Debug.LogError("Cant start game with no player");
			return;
		}
		if(!ArePlayersReady())
		{
			Debug.LogError("Not all players are ready");
			return;
		}

		//no need to send msg => use photon scene loading
		//mainMenu.Photon.Send(EPhotonMsg_MainMenu.Play);
		brainiacs.Scenes.LoadScene(EScene.Loading);
		//brainiacs.Scenes.LoadScene(EScene.Game);
	}

	private bool ArePlayersReady()
	{
		foreach(var p in GetActivatedPlayers())
		{
			if(!p.Info.IsReady)
				return false;
		}
		return true;
	}

	private UIGameSetupPlayerEl GetMyPlayer()
	{
		return players.Find(a => a.IsItMe);
	}

	private List<UIGameSetupPlayerEl> GetActivatedPlayers()
	{
		return players.FindAll(a => a.gameObject.activeSelf);
	}

	private List<UIGameSetupPlayerEl> GetAvailableRemotePlayers()
	{
		return GetActivatedPlayers().FindAll(a => a.IsRemoteFree());
	}

	//private List<UIGameSetupPlayerEl> GetValidPlayers()
	//{
	//	return players.FindAll(a => a.IsValidAndReady());
	//}

	private UIGameSetupPlayerEl GetLocalPlayer()
	{
		return GetActivatedPlayers().Find(a => a.Info.PlayerType == EPlayerType.LocalPlayer);
	}
}
