using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameSetupMain : MainMenuController
{
	[SerializeField] GameObject btnGroupAddPlayer;
	[SerializeField] Button btnAddPlayerAi;
	[SerializeField] Button btnAddPlayerLocal;
	[SerializeField] Button btnAddPlayerRemote;

	[SerializeField] Transform playersHolder;
	[SerializeField] UIGameSetupPlayerEl playerPrefab;
	List<UIGameSetupPlayerEl> players = new List<UIGameSetupPlayerEl>();

	[SerializeField] Toggle gameModeToggleTime;
	[SerializeField] Toggle gameModeToggleScore;
	[SerializeField] Toggle gameModeToggleDeathmatch;
	[SerializeField] UiTextSwapper gameModeValueSwapper;

	[SerializeField] Image mapPreview;
	[SerializeField] UiTextSwapper mapSwapper;


	[SerializeField] Button btnBack;
	[SerializeField] Button btnPlay;

	GameInitInfo gameInitInfo;

	EGameMode selectedGameMode;


	int gameModeValue;
	const int GAME_VALUE_MIN = 1;
	const int GAME_VALUE_MAX = 10;

	EMap selectedMap;

	protected override void Awake()
	{
		btnBack.onClick.AddListener(mainMenu.GameSetup.OnSubMenuBtnBack);
		btnPlay.onClick.AddListener(OnBtnPlay);
		base.Awake();
	}

	private void OnBtnPlay()
	{
		gameInitInfo = new GameInitInfo();
		if(GetLocalPlayer() == null)
		{
			Debug.LogError("Cant start game with no local player");
			return;
		}

		foreach(var p in GetValidPlayers())
		{
			gameInitInfo.players.Add(p.GetInitInfo());
		}
		if(gameInitInfo.players.Count < 1)
		{
			Debug.LogError("Cant start game with no player");
			return;
		}

		gameInitInfo.Map = selectedMap;
		gameInitInfo.Mode = selectedGameMode;
		gameInitInfo.GameModeValue = gameModeValue;

		brainiacs.GameInitInfo = gameInitInfo;
		brainiacs.Scenes.LoadScene(EScene.Loading);
	}

	private List<UIGameSetupPlayerEl> GetActivatedPlayers()
	{
		return players.FindAll(a => a.gameObject.activeSelf);
	}

	private List<UIGameSetupPlayerEl> GetValidPlayers()
	{
		return players.FindAll(a => a.IsValid());
	}

	private UIGameSetupPlayerEl GetLocalPlayer()
	{
		return GetValidPlayers().Find(a => a.PlayerType == EPlayerType.LocalPlayer);
	}

	public void SetActive(bool pValue)
	{
		foreach(var p in players)
		{
			p.gameObject.SetActive(false);
		}

		if(pValue)
		{
			AddPlayer(EPlayerType.LocalPlayer);
		}
		gameObject.SetActive(pValue);
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
		gameModeToggleScore.onValueChanged.AddListener(OnScoreToggled);
		gameModeToggleDeathmatch.onValueChanged.AddListener(OnDeathmatchToggled);

		mapSwapper.Init(Utils.GetStrings(typeof(EMap)), OnMapChanged, 0);
	}

	private void OnMapChanged()
	{
		selectedMap = (EMap)mapSwapper.CurrentIndex;
		mapPreview.sprite = brainiacs.MapManager.GetMapConfig(selectedMap).MapPreview;
	}

	private void OnGameModeValueChanged()
	{
		gameModeValue = GAME_VALUE_MIN + gameModeValueSwapper.CurrentIndex;
		Debug.Log("gameModeValue = " + gameModeValue);
	}

	private void OnDeathmatchToggled(bool pValue)
	{
		if(!pValue)
			return;
		selectedGameMode = EGameMode.Deathmatch;
	}

	private void OnScoreToggled(bool pValue)
	{
		if(!pValue)
			return;
		selectedGameMode = EGameMode.Score;
	}

	private void OnTimeToggled(bool pValue)
	{
		if(!pValue)
			return;
		selectedGameMode = EGameMode.Time;
	}

	private void AddPlayer(EPlayerType pType)
	{
		btnGroupAddPlayer.transform.parent = null;

		bool reActivated = false;

		UIGameSetupPlayerEl addedElement = null;
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
		addedElement.Init(players.IndexOf(addedElement) + 1, pType);


		//todo: check player count (max 4?)
		//keep btnAddPlayer at the end of the list
		btnGroupAddPlayer.transform.parent = playersHolder;

		OnPlayersChanged();
	}


	public void OnPlayersChanged()
	{
		int activatedPlayersCount = GetActivatedPlayers().Count;
		bool isMaxPlayersCount = activatedPlayersCount == 4;
		btnGroupAddPlayer.SetActive(!isMaxPlayersCount);

		if(activatedPlayersCount > 4)
		{
			Debug.LogError("There are more than 4 players");
		}

		bool alreadyHasLocalPlayer = 
			PlatformManager.IsMobile() && GetLocalPlayer() != null;
		btnAddPlayerLocal.gameObject.SetActive(!alreadyHasLocalPlayer);
	}
}
