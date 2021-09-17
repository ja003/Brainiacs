using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonPlayer = Photon.Realtime.Player;

public class UIGameSetupSearch : MainMenuController
{
	[SerializeField] TMP_InputField txtInputGameId;
	[SerializeField] Button btnJoinRandomGame;
	[SerializeField] Text txtStatus;
	[SerializeField] Animator txtStatusAnimator;
	[SerializeField] Button btnBack;
	[SerializeField] Image joinGameIndicator;

	protected override void OnMainControllerAwaken()
	{
		btnJoinRandomGame.onClick.AddListener(OnClickJoinRandomGame);
		txtInputGameId.onValueChanged.AddListener(OnGameIdSet);
		btnBack.onClick.AddListener(OnClickBtnBack);
		SetActive(false);
	}

	private void OnClickBtnBack()
	{
		mainMenu.OnBtnBack();
		SetActive(false);
	}

	string currentGameId;

	private void OnGameIdSet(string pGameId)
	{
		if(pGameId.Length == 0)
			return;

		currentGameId = pGameId;
		Debug.Log($"OnGameIDSet {pGameId}");
		//call join room with delay to give user time to write game id
		DoInTime(() => TryStartSearchForGame(pGameId), 1);
		//txtStatus.text = $"Joining game [{pGameId}]";
		joinGameIndicator.enabled = true;
		btnJoinRandomGame.interactable = false;
	}

	private void TryStartSearchForGame(string pGameId)
	{
		if(currentGameId != pGameId)
		{
			Debug.Log("new game id was set");
			return;
		}
		brainiacs.PhotonManager.JoinRoom(pGameId, mainMenu.SetupMain.OnKickedOut);
	}

	private void OnClickJoinRandomGame()
	{
		Debug.Log($"CountOfRooms = {PhotonNetwork.CountOfRooms}");
		Debug.Log($"CountOfPlayersOnMaster = {PhotonNetwork.CountOfPlayersOnMaster}");
		Debug.Log($"CountOfPlayers = {PhotonNetwork.CountOfPlayers}");

		//Fail is handled in PhotonManager::OnJoinRandomFailed
		DoInTime(() => brainiacs.PhotonManager.JoinRandomRoom(mainMenu.SetupMain.OnKickedOut), 0.5f);
		//brainiacs.PhotonManager.JoinRandomRoom(mainMenu.SetupMain.OnKickedOut);
		//txtStatus.text = "Joining random game";
		joinGameIndicator.enabled = true;
		txtInputGameId.enabled = false;
		btnJoinRandomGame.interactable = false;
	}

	public override void SetActive(bool pValue)
	{
		base.SetActive(pValue);

		btnJoinRandomGame.interactable = pValue;
		txtInputGameId.enabled = pValue;
		joinGameIndicator.enabled = false;
		txtStatus.text = "";
		//Debug.LogError("comment before build");
		//return; //debug

		if(pValue && debug.AutoJoinRandomGame)
			OnClickJoinRandomGame();
	}

	//todo: menu transitions

	internal void OnJoinFailed(short returnCode, string message)
	{
		joinGameIndicator.enabled = false;
		txtStatus.text = message;// "Joining game failed";
		txtStatusAnimator.Rebind();
		btnJoinRandomGame.interactable = true;
		txtInputGameId.enabled = true;
	}

	public void debug_CreateRoom()
	{
		brainiacs.PhotonManager.CreateRoom(4, debug_OnRemotePlayerEnteredRoom, debug_OnRemotePlayerLeftRoom);

	}

	private void debug_OnRemotePlayerEnteredRoom(PhotonPlayer pPlayer)
	{
		Debug.Log("debug_OnRemotePlayerEnteredRoom " + pPlayer);
	}

	private void debug_OnRemotePlayerLeftRoom(PhotonPlayer pPlayer)
	{
		Debug.Log("debug_OnRemotePlayerLeftRoom " + pPlayer);
	}

	public void debug_ReceiveGameinfo()
	{
		GameInitInfo info = new GameInitInfo();
		info.Mode = EGameMode.Deathmatch;

		PlayerInitInfo player = new PlayerInitInfo(
			1, EHero.Currie, "ADA", EPlayerColor.Blue, EPlayerType.LocalPlayer);
		player.PhotonPlayer = PhotonNetwork.LocalPlayer;
		info.UpdatePlayer(player);

		player = new PlayerInitInfo(
			1, EHero.Einstein, "ADA_bot", EPlayerColor.Red, EPlayerType.AI);
		player.PhotonPlayer = PhotonNetwork.LocalPlayer;
		info.UpdatePlayer(player);

		player = new PlayerInitInfo(
			1, EHero.DaVinci, "ADA_remote", EPlayerColor.Yellow, EPlayerType.RemotePlayer);
		player.PhotonPlayer = PhotonNetwork.LocalPlayer;
		info.UpdatePlayer(player);

		player = new PlayerInitInfo(
			1, EHero.Nobel, "ADA_remote2", EPlayerColor.Pink, EPlayerType.RemotePlayer);
		//player.PhotonPlayer = PhotonNetwork.LocalPlayer;
		player.IsReady = true;
		info.UpdatePlayer(player);

		var gameInfoBytes = info.Serialize();
		MainMenu.Instance.Photon.debug_HandleMsg(EPhotonMsg.MainMenu_SyncGameInfo, gameInfoBytes);
	}

}
