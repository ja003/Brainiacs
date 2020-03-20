using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

using PhotonPlayer = Photon.Realtime.Player;

/// <summary>
/// Manages connection to photon server, creating and joining room.
/// TODO: maybe should be on MainMenu and not on Brainiacs?
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
	public Action<PhotonPlayer> OnPlayerEntered;

	// Start is called before the first frame update
	void Start()
	{
		string suffix = Application.isMobilePlatform ? "mobile" : "pc";
		PhotonNetwork.LocalPlayer.NickName = "ADAM_" + suffix;

		Debug.Log("Connecting..");
		PhotonNetwork.ConnectUsingSettings();
	}

	public void JoinRandomRoom()
	{
		Debug.Log("JoinRandomRoom");
		PhotonNetwork.JoinRandomRoom();
	}

	public void CreateRoom(int pMaxPLayers, Action<PhotonPlayer> pOnPlayerEntered)
	{
		Debug.Log("CreateRoom");
		PhotonNetwork.CreateRoom("room1", new RoomOptions { MaxPlayers = (byte)pMaxPLayers }, TypedLobby.Default);
		OnPlayerEntered = pOnPlayerEntered;
	}

	public void LeaveRoom()
	{
		if(PhotonNetwork.CurrentRoom == null)
		{
			Debug.Log("no room to leave");
			return;
		}
		Debug.Log("LeaveRoom");
		PhotonNetwork.LeaveRoom();
	}



	public override void OnConnectedToMaster()
	{
		base.OnConnectedToMaster();
		Debug.Log("Connected to master");

		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();
		Debug.Log("OnJoinedLobby");

		if(DebugData.TestMP)
		{

		}
	}

	public override void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		base.OnCreatedRoom();
	}

	//public bool IsMultiplayer;

	public override void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom");
		base.OnJoinedRoom();
		//IsMultiplayer = true;
		if(!PhotonNetwork.IsMasterClient)
		{
			Debug.Log("Opening main game setup");
			Debug.Log("...maybe after fisrt game info msg?");
			//MainMenu.Instance.GameSetup.OpenMain(false);
		}

	}

	public override void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom");
		//IsMultiplayer = false;
		base.OnLeftRoom();
	}

	public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
	{
		Debug.Log("OnPlayerEnteredRoom " + newPlayer);
		base.OnPlayerEnteredRoom(newPlayer);

		OnPlayerEntered.Invoke(newPlayer);
	}
}
