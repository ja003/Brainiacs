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

	// Start is called before the first frame update
	void Start()
	{
		if(SceneManager.GetActiveScene().name == "S3_Game")
			return;

		PhotonNetwork.AutomaticallySyncScene = true;

		string suffix = Application.isMobilePlatform ? "mobile" : "pc";
#if UNITY_EDITOR
		suffix = "editor";
#endif

		PhotonNetwork.LocalPlayer.NickName = "ADAM_" + suffix;

		Debug.Log("Connecting..");
		PhotonNetwork.ConnectUsingSettings();
	}

	public void JoinRandomRoom()
	{
		Debug.Log("JoinRandomRoom");
		PhotonNetwork.JoinRandomRoom();
	}

	public void JoinRoom(string pName)
	{
		Debug.Log("JoinRoom " + pName);
		PhotonNetwork.JoinRoom(pName);
	}

	public string CreateRoom(int pMaxPLayers, Action<PhotonPlayer> pOnPlayerEntered)
	{
		string roomName = GenerateRoomName();

		Debug.Log("CreateRoom " + roomName);

		PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)pMaxPLayers }, TypedLobby.Default);
		OnPlayerEntered = pOnPlayerEntered;
		return roomName;
	}

	private string GenerateRoomName()
	{
		int index = UnityEngine.Random.Range(1, 100);
		return "room_" + index;
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
		//if(!PhotonNetwork.IsMasterClient)
		//{
		//	Debug.Log("Opening main game setup");
		//	Debug.Log("...maybe after fisrt game info msg?");
		//	//MainMenu.Instance.GameSetup.OpenMain(false);
		//}

	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		base.OnJoinRandomFailed(returnCode, message);
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
		Debug.Log("OnDisconnected");
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
	/// If single player => true
	/// </summary>
	public bool IsMaster()
	{
		if(!Brainiacs.Instance.GameInitInfo.IsMultiplayer())
			return true;

		return PhotonNetwork.IsMasterClient;
	}
}
