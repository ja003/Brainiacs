using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class PlayerManager : GameController
{
	public List<Player> Players { get; private set; } = new List<Player>();

	[SerializeField] private Player playerPrefab = null;
	[SerializeField] private PlayerSorter playerSorter = null;

	public Vector2 PLAYER_SIZE => playerPrefab.Collider.size;

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
			game.Map.SetOnActivated(() =>
					SpawnPlayers(brainiacs.GameInitInfo.Players));
		}
	}

	public void btn_debugSpawnPlayer()
	{
		SpawnPlayer(brainiacs.GameInitInfo.Players[0], false);
	}

	public void btn_debugSyncPLayersInfo()
	{
		//foreach(var p in Players)
		//{
		//	p.debug_SendInitInfo();
		//}
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

			if(DebugData.LocalImage && spawnedPlayer.LocalImage == null)
			{
				spawnedPlayer.LocalImage = SpawnPlayer(playerInfo, true);
				spawnedPlayer.LocalImage.LocalImageOwner = spawnedPlayer;
				AddPlayer(spawnedPlayer.LocalImage);
			}

			AddPlayer(spawnedPlayer);
			spawnedPlayer.LocalImage?.OnReceivedInitInfo(playerInfo, true);
		}

		playerSorter.SetPlayers(Players);

		//invoke on activated
		Activate();
		arePlayersSpawned = true;
	}

	/// <summary>
	/// Only master spawn players
	/// </summary>
	private Player SpawnPlayer(PlayerInitInfo pPlayerInfo, bool pIsLocalImage)
	{
		//Vector2 spawnPosition = game.MapController.ActiveMap.GetSpawnPoint().position;
		Vector2 spawnPosition = game.Map.ActiveMap.
			GetSpawnPoint(pPlayerInfo.Number).position;
		if(pIsLocalImage)
			spawnPosition += Vector2.down;

		GameObject instance = InstanceFactory.Instantiate(playerPrefab.gameObject, spawnPosition);

		Player playerInstance = instance.GetComponent<Player>();

		//playerInstance.transform.parent = transform; //handled in Pool

		//moved to Player::SetInfo
		//playerInstance.gameObject.name = "Player_" + pPlayerInfo.Name + (pIsLocalImage ? "_LI" : "");

		

		bool debug_spwan = false;
		if(debug_spwan)
		{
			if(pPlayerInfo.Name == DebugData.GetPlayerName(1))
				spawnPosition = new Vector2(-3.3f, 3);
			//spawnPosition = Vector2.down;

			if(pPlayerInfo.Name == DebugData.GetPlayerName(2))
				spawnPosition = new Vector2(-1.8f, 2.3f);
			//spawnPosition = Vector2.zero;

			if(pPlayerInfo.Name == DebugData.GetPlayerName(3))
				spawnPosition = Vector2.down;
		}

		//OnAllPlayersAdded += () => playerInstance.SetInfo(pPlayerInfo, spawnPosition);
		playerInstance.SetInfo(pPlayerInfo, pIsLocalImage, spawnPosition);

		//AddPlayer(playerInstance);

		return playerInstance;
	}

	//1 - spawn, 2 - add player
	public ActionControl OnAllPlayersAdded = new ActionControl();

	/// <summary>
	/// Use this instead of: Players.Add()
	/// </summary>
	public void AddPlayer(Player pPlayer)
	{
		if(pPlayer.ai.IsTmp)
		{
			//Debug.Log("Skip tmp ai");
			//tmp AIs (Tesla clone) are not registered as active players
			return;
		}

		Players.Add(pPlayer);
		//Debug.Log("Add player " + pPlayer);
		int playersCount = Players.FindAll(a => !a.IsLocalImage).Count;

		if(playersCount == brainiacs.GameInitInfo.Players.Count)
		{
			if(pPlayer.IsLocalImage)
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

		game.Map.SetOnActivated(() =>
				SpawnPlayers(brainiacs.GameInitInfo.Players));
	}

	public Player GetPlayer(PhotonPlayer pPhotonPlayer)
	{
		return Players.Find(a => ((PlayerPhotonController)a.Photon).PhotonPlayer == pPhotonPlayer);
	}

	internal Player GetPlayer(int pPlayerNumber)
	{
		return Players.Find(a => a.InitInfo.Number == pPlayerNumber);
	}

	/// <summary>
	/// Returns all players aother than the given player sorted by the distance to that player
	/// </summary>
	public List<Player> GetOtherPlayers(Player pOtherThan, bool pHasToBeAlive, bool pSortByDistance = true)
	{
		List<Player> otherPlayers = Players.Where(a => !a.Equals(pOtherThan)).ToList();
		if(pHasToBeAlive) //only visual.IsDying is shared to player images
			otherPlayers = otherPlayers.Where(a => !a.Visual.IsDying).ToList();
		if(pSortByDistance)
		{
			otherPlayers.Sort((a, b) =>
				Vector2.Distance(pOtherThan.transform.position, a.transform.position)
					.CompareTo(Vector2.Distance(pOtherThan.transform.position, b.transform.position)));
		}
		return otherPlayers;
	}

	public Player GetClosestPlayerTo(Player pPlayer, bool pHasToBeAlive = true)
	{
		var otherPlayers = GetOtherPlayers(pPlayer, pHasToBeAlive);
		return otherPlayers.Count > 0 ? otherPlayers[0] : null;
	}

	/// <summary>
	/// Return
	/// </summary>
	/// <param name="pOtherThan"></param>
	/// <returns></returns>
	//public List<Player> GetOtherPlayersAlive(Player pOtherThan)
	//{
	//	List<Player> otherPlayers = GetOtherPlayers(pOtherThan).Where(a => !a.Visual.IsDying).ToList();

	//	return otherPlayers;
	//}

}
