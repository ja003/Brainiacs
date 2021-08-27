using FlatBuffers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ExtensionMethods;

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
	[SerializeField] GameObject hintBtnReady = null;
	[SerializeField] Button btnAllowJoin = null;

	[SerializeField] Button btnCopyGameId;
	[SerializeField] TextMeshProUGUI txtGameId;

	[Header("OTHER")]
	[SerializeField] TextMeshProUGUI txtGameType;
	[SerializeField] Animator txtGameTypeAnim;

	GameInitInfo gameInitInfo => brainiacs.GameInitInfo;


	const int GAME_VALUE_MIN = 1;
	const int GAME_VALUE_MAX = 10;

	//there is at least 1 remote player defined
	bool isSetAsMPGame;

	protected override void Awake()
	{
		btnBack.onClick.AddListener(OnBtnBack);
		btnPlay.onClick.AddListener(OnBtnPlay);
		btnReady.onClick.AddListener(OnBtnReady);
		btnAllowJoin.onClick.AddListener(OnBtnAllowJoin);
		btnCopyGameId.onClick.AddListener(OnBtnCopyGameId);
		brainiacs.PhotonManager.OnPlayerLeft += OnPlayerLeft;
		base.Awake();
	}

	private void OnBtnBack()
	{
		KickOtherPlayers();
		brainiacs.PhotonManager.LeaveRoom();
		brainiacs.GameInitInfo = null;

		//setupInit.SetActive(true);
		//setupSearch.SetActive(false);
		SetActive(false);
		mainMenu.OnBtnBack();
	}

	private void Update()
	{
		btnAllowJoin.interactable = PhotonNetwork.IsConnectedAndReady && !isJoinAllowed && isSetAsMPGame;
		btnReady.interactable = PhotonNetwork.IsConnectedAndReady;
		btnPlay.interactable = isMultiplayer ? PhotonNetwork.IsConnectedAndReady : true;
	}

	//store last master player so we can detect if he disconnects (see OnPlayerLeft)
	PhotonPlayer masterPlayer;

	internal void SetGameInfo(GameInitInfo pGameInfo)
	{
		for(int i = 0; i < pGameInfo.Players.Count; i++)
		{
			PlayerInitInfo player = pGameInfo.Players[i];
			AddPlayer(player);

			if(player.PhotonPlayer.IsMasterClient)
				masterPlayer = player.PhotonPlayer;
		}
		Debug.Log($"masterPlayer {masterPlayer}");

		EGameMode mode = pGameInfo.Mode;
		gameModeToggleTime.isOn = mode == EGameMode.Time;
		gameModeToggleScore.isOn = mode == EGameMode.Score;
		gameModeToggleDeathmatch.isOn = mode == EGameMode.Deathmatch;

		mapSwapper.SetValue((int)pGameInfo.Map);

		gameModeValueSwapper.SetNumberValue(pGameInfo.GameModeValue);

		//set just game values. 
		//player info is determined from UI elements.
		brainiacs.GameInitInfo.SetGameValues(pGameInfo); //needed to set map

		//update needed on client who just joined
		UpdateGameType();

		if(debug.InstaReady)
		{
			Debug.Log("debug: insta ready");
			UIGameSetupPlayerEl myPLayerEl = GetMyPlayer();
			bool isReady = myPLayerEl.Info.IsReady;
			if(!isReady)
				OnBtnReady();
		}
	}

	//if this mode was opened in master mode
	public bool IsMaster;
	//just to detect if it was master who canceled the game or player was kicked out
	float lastTimeWasMaster;

	private void SetMenuMode(bool pIsMaster)
	{
		if((IsMaster && !pIsMaster) || pIsMaster)
			lastTimeWasMaster = Time.time;
		IsMaster = pIsMaster;

		btnAllowJoin.gameObject.SetActive(pIsMaster);
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

		//client always shows room info so he can share it 
		if(pValue && !IsMaster)
		{
			txtGameId.text = game_id_prefix + PhotonNetwork.CurrentRoom.Name;
			btnCopyGameId.gameObject.SetActive(true);
		}

		//reset elements
		foreach(var p in players)
		{
			p.gameObject.SetActive(false);
		}

		if(pValue)
		{
			if(pIsMaster)
			{
				//reset game info
				brainiacs.GameInitInfo = new GameInitInfo();

				AddPlayer(EPlayerType.LocalPlayer);
				if(debug.ExtraPlayerAtStart != EPlayerType.None)
					AddPlayer(debug.ExtraPlayerAtStart);

				//set default values
				OnTimeToggled(true);
				OnScoreToggled(true);
				OnMapChanged();
				OnGameModeValueChanged();
			}

			//this resets ready btn after every sync
			////reset
			//SetBtnReadyState(false);

			if(!IsActive())
			{
				//show after open menu anim
				DoInTime(() => mainMenu.InfoMessenger.Show("WELCOME!"), 1);
			}
		}
		else
		{
			//reset
			SetBtnReadyState(false);
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
		if(debug.GameValue > 0)
			gameModeValueSwapper.SetNumberValue(debug.GameValue); //debug
		OnTimeToggled(true);

		gameModeToggleScore.onValueChanged.AddListener(OnScoreToggled);
		gameModeToggleDeathmatch.onValueChanged.AddListener(OnDeathmatchToggled);

		//DEBUG game mode
		//gameModeToggleScore.isOn = true;
		//OnGameModeToggled(true, EGameMode.Score);

		mapSwapper.Init(Utils.GetStrings(typeof(EMap)), OnMapChanged, 0);
	}

	protected override void OnMainControllerActivated()
	{
		SetActive(false);
	}

	/// MULTIPLAYER

	bool isJoinAllowed;
	const string game_id_prefix = "game id: ";

	private void OnBtnAllowJoin()
	{
		isJoinAllowed = true;
		//todo: check if any remote + check max players
		int playersCount = GetActivatedPlayers().Count;
		string roomName = brainiacs.PhotonManager.CreateRoom(playersCount,
			OnRemotePlayerEnteredRoom, OnRemotePlayerLeftRoom);

		btnGroupAddPlayer.SetActive(false);

		btnCopyGameId.gameObject.SetActive(true);
		txtGameId.text = game_id_prefix + roomName;
		btnAllowJoin.gameObject.SetActive(false);
	}

	private void OnBtnCopyGameId()
	{
		string gameId = txtGameId.text.Replace(game_id_prefix, "");
		UniClipboard.SetText(gameId);
		Debug.Log($"{gameId} copied to clipboard");
		string message = $"Game ID {gameId.Colorify(Color.red)} copied to clipboard" + Environment.NewLine
			+ "share it with your friend!";
		mainMenu.InfoMessenger.Show(message);
	}



	private void debug_OnBtnSyncInfo()
	{
		Debug.LogError("DEBUG sync");
		brainiacs.SyncGameInitInfo();
	}


	private void OnMapChanged()
	{
		brainiacs.GameInitInfo.Map = (EMap)mapSwapper.CurrentIndex;
		mapPreview.sprite = brainiacs.MapManager.
			GetMapConfig(brainiacs.GameInitInfo.Map).MapPreview;

		brainiacs.SyncGameInitInfo();
	}

	private void OnGameModeValueChanged()
	{
		gameInitInfo.GameModeValue = GAME_VALUE_MIN + gameModeValueSwapper.CurrentIndex;
		//Debug.Log("gameModeValue = " + gameInitInfo.GameModeValue);

		brainiacs.SyncGameInitInfo();
	}

	private void OnDeathmatchToggled(bool pValue)
	{
		OnGameModeToggled(pValue, EGameMode.Deathmatch);
	}

	private void OnScoreToggled(bool pValue)
	{
		//Debug.Log("OnScoreToggled " + pValue);
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
		brainiacs.SyncGameInitInfo();
	}

	//remote
	private void AddPlayer(PlayerInitInfo pPlayer)
	{
		UIGameSetupPlayerEl addedElement = AddPlayerElement();
		addedElement.Init(pPlayer);
	}

	[SerializeField] Tutorial tutorialSelectHero;
	bool isFirstPlayerAdded;

	/// <summary>
	/// MASTER
	/// Called on btnAddPlayer(AI/Local/Remote) click
	/// </summary>
	private void AddPlayer(EPlayerType pType)
	{
		if(pType == EPlayerType.None)
		{
			Debug.LogError("Cant add player of type None");
			return;
		}

		btnGroupAddPlayer.transform.SetParent(null);

		UIGameSetupPlayerEl addedElement = AddPlayerElement();
		addedElement.Init(players.IndexOf(addedElement) + 1, pType);

		if(!isFirstPlayerAdded)
		{
			tutorialSelectHero.SetFocusedObject(addedElement.color.gameObject);
			tutorialSelectHero.SetFocusedObject(addedElement.heroSwapper.gameObject);

			tutorialSelectHero.SetTargetBtn(addedElement.heroSwapper.btnNext);
			//only 1 button is used now
			//tutorialSelectHero.SetTargetBtn(addedElement.heroSwapper.btnPrevious);
			tutorialSelectHero.GetComponent<UiCopyPosition>().copyFrom = addedElement.GetComponent<RectTransform>();
		}

		//todo: check player count (max 4?)
		//keep btnAddPlayer at the end of the list
		btnGroupAddPlayer.transform.SetParent(playersHolder);

		OnPlayersChanged();
		isFirstPlayerAdded = true;
	}


	internal void UpdatePlayer(PlayerInitInfo pPlayerInfo)
	{
		GetPlayerEl(pPlayerInfo.Number)?.UpdateInfo(pPlayerInfo);
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

		UpdateGameType();
		brainiacs.SyncGameInitInfo();
	}

	/// <summary>
	/// Show info if game is multi/single player.
	/// Set visibility of join-buttons
	/// </summary>
	private void UpdateGameType()
	{
		isSetAsMPGame = false;
		foreach(var p in GetActivatedPlayers())
		{
			if(p.Info.PlayerType == EPlayerType.RemotePlayer)
			{
				isSetAsMPGame = true;
				break;
			}
		}
		//Debug.Log("UpdateGameType " + isSetAsMPGame);
		string gameTypeStr = isSetAsMPGame ? "NETWORK GAME" : "LOCAL GAME";
		bool isChange = gameTypeStr != txtGameType.text;
		txtGameType.text = gameTypeStr;
		//if game type changed => highlight it (by anim)
		if(isChange)
			txtGameTypeAnim.Rebind();

		//only master changes visibility of join-buttons
		if(IsMaster)
		{
			btnAllowJoin.gameObject.SetActive(isSetAsMPGame && !isJoinAllowed);
			btnCopyGameId.gameObject.SetActive(isSetAsMPGame && isJoinAllowed);
		}
	}

	private void OnBtnReady()
	{
		UIGameSetupPlayerEl myPLayerEl = GetMyPlayer();
		bool isReady = myPLayerEl.Info.IsReady;
		myPLayerEl.SetReady(!isReady);
		SetBtnReadyState(!isReady);
	}

	private void SetBtnReadyState(bool pIsReady)
	{
		btnReadyText.text = pIsReady ? "NOT READY" : "READY";
		hintBtnReady.SetActive(!pIsReady);
	}

	private void OnBtnPlay()
	{
		if(GetLocalPlayer() == null)
		{
			mainMenu.InfoMessenger.Show(
				"Cant start game with no local player", DebugInfoMsg.Error);
			return;
		}

		if(gameInitInfo.Players.Count < 1)
		{
			mainMenu.InfoMessenger.Show(
				"Cant start game with no player", DebugInfoMsg.Error);
			return;
		}
		if(!ArePlayersReady())
		{
			mainMenu.InfoMessenger.Show(
				"Not all players are ready", DebugInfoMsg.Error);
			return;
		}

		//todo: map info is not yet synced right after change. 
		//sync it now, clients need map info
		//brainiacs.SyncGameInitInfo(); //fixed?

		Debug.Log(brainiacs.GameInitInfo);

		brainiacs.SyncGameInitInfo();
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

	public List<UIGameSetupPlayerEl> GetActivatedPlayers()
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

	///-------------------- MULTIPLAYER --------------------///
	///
	internal void KickOtherPlayers()
	{
		if(!IsMaster)
			return;

		List<PhotonPlayer> players = GetActivatedPlayers().Select(i => i.Info.PhotonPlayer).ToList();
		brainiacs.PhotonManager.KickPlayers(players, true);
	}

	internal void OnKickedOut()
	{
		bool wasMaster = Time.time - lastTimeWasMaster < 5;
		Debug.Log("OnKickedOut " + (IsMaster || wasMaster));
		if(IsMaster || wasMaster)
			return;

		mainMenu.InfoMessenger.Show("You were kicked out!");
		DoInTime(OnBtnBack, 2);
	}

	private void OnRemotePlayerEnteredRoom(PhotonPlayer pPlayer)
	{
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

	private void OnRemotePlayerLeftRoom(PhotonPlayer pPlayer)
	{
		foreach(UIGameSetupPlayerEl playerEl in GetActivatedPlayers())
		{
			if(playerEl.Info.PhotonPlayer == pPlayer)
			{
				playerEl.OnRemoteDisconnected(pPlayer);
				//mainMenu.InfoMessenger.Show($"Player {pPlayer} has left");
				return;
			}
		}
		Debug.LogError($"Player {pPlayer} left room but was not found in active players");
	}

	/// <summary>
	/// Used to detect if master client left.
	/// It would be too complicated to handle change of master => kick player.
	/// </summary>
	private void OnPlayerLeft(PhotonPlayer pPlayer)
	{
		Debug.Log($"OnPlayerLeft {pPlayer} - {pPlayer.IsMasterClient}");
		if(pPlayer == masterPlayer)
		{
			Debug.Log("it was master => KICK");
			OnKickedOut();
		}
	}
}
