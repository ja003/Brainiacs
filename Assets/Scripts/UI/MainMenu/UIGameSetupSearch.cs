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
		btnJoinRandomGame.onClick.AddListener(OnJoinRandomGame);
		txtInputRoomName.onValueChanged.AddListener(OnRoomNameSet);
	}

	private void OnRoomNameSet(string pRoomName)
	{
		Debug.Log($"OnGameIDSet {pRoomName}");
		brainiacs.PhotonManager.JoinRoom(pRoomName);
		txtStatus.text = "Joining room " + pRoomName;
	}

	private void OnJoinRandomGame()
	{
		brainiacs.PhotonManager.JoinRandomRoom();
		txtStatus.text = "Joining random room";
	}

	protected override void OnSetActive(bool pValue)
	{
		txtStatus.text = "";
		//Debug.LogError("comment before build");
		//return; //debug

		//if(pValue)
		//	brainiacs.PhotonManager.JoinRandomRoom();
	}

	public void debug_CreateRoom()
	{
		brainiacs.PhotonManager.CreateRoom(4, OnRemotePlayerEnteredRoom);

	}
	private void OnRemotePlayerEnteredRoom(PhotonPlayer pPlayer)
	{
		Debug.Log("OnRemotePlayerEnteredRoom " + pPlayer);
	}

	public void debug_ReceiveGameinfo()
	{
		GameInitInfo info = new GameInitInfo();
		info.Mode = EGameMode.Deathmatch;

		PlayerInitInfo player = new PlayerInitInfo(
			1, EHero.Currie, "ADA", EPlayerColor.Blue, EPlayerType.LocalPlayer);
		player.PhotonPlayer = PhotonNetwork.LocalPlayer;
		info.AddPlayer(player);

		player = new PlayerInitInfo(
			1, EHero.Einstein, "ADA_bot", EPlayerColor.Red, EPlayerType.AI);
		player.PhotonPlayer = PhotonNetwork.LocalPlayer;
		info.AddPlayer(player);

		player = new PlayerInitInfo(
			1, EHero.DaVinci, "ADA_remote", EPlayerColor.Yellow, EPlayerType.RemotePlayer);
		player.PhotonPlayer = PhotonNetwork.LocalPlayer;
		info.AddPlayer(player);

		player = new PlayerInitInfo(
			1, EHero.Nobel, "ADA_remote2", EPlayerColor.Pink, EPlayerType.RemotePlayer);
		//player.PhotonPlayer = PhotonNetwork.LocalPlayer;
		player.IsReady = true;
		info.AddPlayer(player);

		var gameInfoBytes = info.Serialize();
		MainMenu.Instance.Photon.debug_HandleMsg(EPhotonMsg.MainMenu_SyncGameInfo, gameInfoBytes);
	}
}
