using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

using PhotonPlayer = Photon.Realtime.Player;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages connection to photon server, creating and joining room.
/// TODO: maybe should be on MainMenu and not on Brainiacs?
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
	public Action<PhotonPlayer> OnPlayerEntered;
	public Action<PhotonPlayer> OnPlayerLeft;
	public Action OnKickedOut; //called eg when client gets kicked out of room

	void Start()
	{
		if(SceneManager.GetActiveScene().name == "S3_Game")
			return;

		PhotonNetwork.AutomaticallySyncScene = true;

		PhotonNetwork.ConnectUsingSettings();
		//GameVersion has to be set after calling ConnectUsingSettings
		//https://forum.photonengine.com/discussion/13341/restricting-joining-room-with-non-matching-game-version
		PhotonNetwork.GameVersion = Application.version;
		//Debug.Log($"Connecting. {PhotonNetwork.GameVersion}");
		PhotonNetwork.LocalPlayer.NickName = PlayerNameManager.GetPlayerName(1);
	}

	/// <summary>
	/// Reset all actions.
	/// Actions should be specific for each scene.
	/// </summary>
	public void OnSceneChange()
	{
		OnPlayerEntered = null;
		OnPlayerLeft = null;
		OnKickedOut = null;
	}

	public void JoinRandomRoom(Action pOnKickedOut)
	{
		OnKickedOut = pOnKickedOut;
		//Debug.Log("JoinRandomRoom");
		PhotonNetwork.JoinRandomRoom();
	}


	public bool JoinRoom(string pName, Action pOnKickedOut)
	{
		OnKickedOut = pOnKickedOut;
		//Debug.Log("JoinRoom " + pName);
		return PhotonNetwork.JoinRoom(pName);
	}

	public string CreateRoom(int pMaxPLayers, Action<PhotonPlayer> pOnPlayerEntered, Action<PhotonPlayer> pOnPlayerLeft)
	{
		string roomName = GenerateRoomName();
		//Debug.Log($"CreateRoom {roomName}");
		PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)pMaxPLayers }, TypedLobby.Default);
		OnPlayerEntered = pOnPlayerEntered;
		OnPlayerLeft = pOnPlayerLeft;
		return roomName;
	}

	private string GenerateRoomName()
	{
		int index = UnityEngine.Random.Range(1, 100);
		return "" + index;
		//we dont want "room" in game id
		//return "room_" + index;
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

	internal void KickPlayers(List<PhotonPlayer> pPlayers, bool pIgnoreLocal)
	{
		foreach(var p in pPlayers)
		{
			if(p == null || (pIgnoreLocal && p.IsLocal))
				continue;

			Debug.Log("Kick player " + p);
			PhotonNetwork.CloseConnection(p);
		}
	}

	public override void OnConnectedToMaster()
	{
		base.OnConnectedToMaster();
		Debug.Log($"Connected to master {PhotonNetwork.AppVersion} | {PhotonNetwork.NetworkingClient.AppVersion}");


		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();
		//Debug.Log("OnJoinedLobby");
	}

	public override void OnCreatedRoom()
	{
		//Debug.Log("OnCreatedRoom");
		base.OnCreatedRoom();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		MainMenu.Instance.SetupSearch.OnJoinFailed(returnCode, message);
		base.OnJoinRandomFailed(returnCode, message);
	}
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		MainMenu.Instance.SetupSearch.OnJoinFailed(returnCode, message);
		base.OnJoinRoomFailed(returnCode, message);
	}

	public override void OnLeftRoom()
	{
		//Debug.Log("OnLeftRoom");
		OnKickedOut?.Invoke();
		//IsMultiplayer = false;
		base.OnLeftRoom();
	}

	public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
	{
		//Debug.Log("OnPlayerEnteredRoom " + newPlayer);
		base.OnPlayerEnteredRoom(newPlayer);

		OnPlayerEntered?.Invoke(newPlayer);
	}

	public override void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
	{
		Debug.Log("OnPlayerLeftRoom " + otherPlayer);
		base.OnPlayerLeftRoom(otherPlayer);

		OnPlayerLeft?.Invoke(otherPlayer);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		//Debug.Log("OnDisconnected");
		StartCoroutine(TryReconnect());
	}

	private IEnumerator TryReconnect()
	{
		if(PhotonNetwork.IsConnected)
			yield return null;

		yield return new WaitForSeconds(1);

		Debug.Log("reconnect");
		PhotonNetwork.ConnectToBestCloudServer();
	}

	/// <summary>
	/// Am I master client?
	/// </summary>
	public bool IsMaster()
	{
		return Brainiacs.Instance.GameInitInfo.IsMaster();

		//if(!Brainiacs.Instance.GameInitInfo.IsMultiplayer())
		//	return true;

		//return PhotonNetwork.IsMasterClient;
	}
}
