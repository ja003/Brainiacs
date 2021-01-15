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
	[SerializeField] TMP_InputField txtInputRoomName;
	[SerializeField] Button btnJoinRandomGame;
	[SerializeField] Text txtStatus;

	protected override void OnMainControllerAwaken()
	{
		btnJoinRandomGame.onClick.AddListener(OnClickJoinRandomGame);
		txtInputRoomName.onValueChanged.AddListener(OnRoomNameSet);
	}

	private void OnRoomNameSet(string pRoomName)
	{
		Debug.Log($"OnGameIDSet {pRoomName}");
		brainiacs.PhotonManager.JoinRoom(pRoomName, mainMenu.GameSetup.SetupMain.OnKickedOut);
		txtStatus.text = "Joining room " + pRoomName;
	}

	private void OnClickJoinRandomGame()
	{
		brainiacs.PhotonManager.JoinRandomRoom(mainMenu.GameSetup.SetupMain.OnKickedOut);
		txtStatus.text = "Joining random room";
	}

	protected override void OnSetActive(bool pValue)
	{
		txtStatus.text = "";
		//Debug.LogError("comment before build");
		//return; //debug

		if(pValue && debug.AutoJoinRandomRoom)
			OnClickJoinRandomGame();
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
