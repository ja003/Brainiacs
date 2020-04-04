using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class PlayerManager : GameController
{
	public List<Player> Players { get; private set; } = new List<Player>();

	[SerializeField]
	private Player playerPrefab;

	[SerializeField]
	private PlayerSorter playerSorter;


	protected override void OnMainControllerAwaken()
	{
		if(PhotonNetwork.IsMasterClient)
		{
			loadedPlayers.Add(PhotonNetwork.LocalPlayer);
		}
	}

	protected override void OnMainControllerActivated()
	{


		//single player => spawn here
		//MP => spawn in OnRemotePlayerLoadedScene
		if(!brainiacs.GameInitInfo.IsMultiplayer())
		{
			//Debug.Log("Spawn players SP");
			game.MapController.SetOnActivated(() =>
					SpawnPlayers(brainiacs.GameInitInfo.Players));
		}
	}

	public void btn_debugSpawnPlayer()
	{
		SpawnPlayer(brainiacs.GameInitInfo.Players[0], false);
	}

	public void btn_debugSyncPLayersInfo()
	{
		foreach(var p in Players)
		{
			p.Network.debug_SendInitInfo();
		}
	}



	bool arePlayersSpawned;
	private void SpawnPlayers(List<PlayerInitInfo> pPlayersInfo)
	{
		if(arePlayersSpawned)
		{
			//todo: for some reason the PC instance (not in unity)
			//sends LoadedScene 2x
			Debug.LogError("Players already spawned");
			return;
		}

		//Debug.Log("Spawn players");

		foreach(PlayerInitInfo playerInfo in pPlayersInfo)
		{
			Player spawnedPlayer = SpawnPlayer(playerInfo, false);

			if(DebugData.LocalRemote && spawnedPlayer.LocalRemote == null)
			{
				spawnedPlayer.LocalRemote = SpawnPlayer(playerInfo, true);
				//spawnedPlayer.LocalRemote.gameObject.name += "_remote";
				//spawnedPlayer.LocalRemote.transform.position =
				//	spawnedPlayer.transform.position + Vector3.down;

				AddPlayer(spawnedPlayer.LocalRemote);
			}

			AddPlayer(spawnedPlayer);
		}

		playerSorter.SetPlayers(Players);

		//invoke on activated
		Activate();
		arePlayersSpawned = true;
	}

	/// <summary>
	/// Only master spawn players
	/// </summary>
	private Player SpawnPlayer(PlayerInitInfo pPlayerInfo, bool pIsLocalRemote)
	{
		GameObject instance = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
		Player playerInstance = instance.GetComponent<Player>();

		playerInstance.transform.parent = transform;

		playerInstance.gameObject.name = "Player_" + pPlayerInfo.Name + (pIsLocalRemote ? "_LR" : "");

		//Vector3 spawnPosition = game.MapController.ActiveMap.GetSpawnPoint().position;
		Vector3 spawnPosition = game.MapController.ActiveMap.
			GetSpawnPoint(pPlayerInfo.Number).position;
		if(pIsLocalRemote)
			spawnPosition += Vector3.down;

		bool debug_spwan = false;
		if(debug_spwan)
		{
			if(pPlayerInfo.Name == DebugData.GetPlayerName(1))
				spawnPosition = new Vector3(-3.3f, 3, 0);
			//spawnPosition = Vector3.down;

			if(pPlayerInfo.Name == DebugData.GetPlayerName(2))
				spawnPosition = new Vector3(-1.8f, 2.3f, 0);
			//spawnPosition = Vector3.zero;

			if(pPlayerInfo.Name == DebugData.GetPlayerName(3))
				spawnPosition = Vector3.down;
		}

		//OnAllPlayersAdded += () => playerInstance.SetInfo(pPlayerInfo, spawnPosition);
		playerInstance.SetInfo(pPlayerInfo, pIsLocalRemote, spawnPosition);

		//AddPlayer(playerInstance);

		return playerInstance;
	}

	//public Action OnAllPlayersAdded;

	public ActionControl OnAllPlayersAdded = new ActionControl();

	/// <summary>
	/// Use this instead of: Players.Add()
	/// </summary>
	public void AddPlayer(Player pPlayer)
	{
		Players.Add(pPlayer);
		//Debug.Log("Add player " + pPlayer);
		int playersCount = Players.FindAll(a => !a.IsLocalRemote).Count;

		if(playersCount == brainiacs.GameInitInfo.Players.Count)
		{
			if(pPlayer.IsLocalRemote)
				return;

			if(allPlayersAdded)
			{
				Debug.LogError("OnAllPlayersAdded already called");
				return;
			}

			//foreach(var player in Players)
			//{
			//	player.Init();
			//}
			//Debug.Log("All players added");
			OnAllPlayersAdded.Invoke();
			//OnAllPlayersAdded = null;
			allPlayersAdded = true;
		}
	}
	bool allPlayersAdded;

	List<PhotonPlayer> loadedPlayers = new List<PhotonPlayer>();
	internal void OnRemotePlayerLoadedScene(PhotonPlayer pPlayer)
	{
		Debug.Log("OnRemotePlayerLoadedScene" + pPlayer);
		loadedPlayers.Add(pPlayer);

		foreach(var player in brainiacs.GameInitInfo.Players)
		{
			var foundPlayer = loadedPlayers.Find(a => a.ActorNumber == player.PhotonPlayer.ActorNumber);
			if(foundPlayer == null)
			{
				Debug.Log("Not all players loaded yet");
				return;
			}
		}
		Debug.Log("All players loaded");

		//map has to be activated first

		//player objects spawn only on master
		if(brainiacs.GameInitInfo.IsMultiplayer() && !PhotonNetwork.IsMasterClient)
		{
			Debug.Log("Not MasterClient => dont spawn");
			return;
		}

		game.MapController.SetOnActivated(() =>
				SpawnPlayers(brainiacs.GameInitInfo.Players));
	}

	public Player GetPlayer(PhotonPlayer pPhotonPlayer)
	{
		return Players.Find(a => a.Network.PhotonPlayer == pPhotonPlayer);
	}

	internal Player GetPlayer(int pPlayerNumber)
	{
		return Players.Find(a => a.InitInfo.Number == pPlayerNumber);
	}

}
