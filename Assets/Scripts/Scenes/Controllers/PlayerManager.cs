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
	[SerializeField] public PlayerSorter playerSorter = null;

	public Vector2 PLAYER_SIZE => playerPrefab.Collider.size;

	public bool ArePlayersSpawned;
	public bool AreAllPlayersAdded;

	protected override void OnMainControllerAwaken()
	{
		if(PhotonNetwork.IsMasterClient)
		{
			loadedPlayers.Add(PhotonNetwork.LocalPlayer);
		}
		brainiacs.PhotonManager.OnPlayerLeft += OnPlayerLeftRoom;
	}

	public void debug_OnPlayerLeftRoom()
	{
		Debug.Log("debug_OnPlayerLeftRoom");
		OnPlayerLeftRoom(GetPlayer(1).InitInfo.PhotonPlayer);
	}

	/// <summary>
	/// Player disconnected -> remove him.
	/// todo: change to AI - might be problematic to sync this
	/// </summary>
	private void OnPlayerLeftRoom(PhotonPlayer pPlayer)
	{
		Player leaver = GetPlayer(pPlayer);
		//if(PhotonNetwork.IsMasterClient) //on all..right?
		game.InfoMessenger.Show($"Player {leaver.InitInfo.GetName(true)} has <b>left:(</b>");

		/*
		DoInTime(() => ChangeToAi(leaver), 1f);

		void ChangeToAi(Player pLeaver)
		{
			//PlayerInitInfo info = pLeaver.InitInfo;
			//info.PlayerType = EPlayerType.AI;
			//pLeaver.SetInfo(info, false);

			//not working in MP
			pLeaver.InitInfo.PlayerType = EPlayerType.AI;
			pLeaver.ai.Init();
		}

		return;*/

		//for now we change player to ai

		DoInTime(() => RemovePlayer(leaver), 1);

		void RemovePlayer(Player pLeaver)
		{
			pLeaver.Health.DoEliminateEffect();
			pLeaver.ReturnToPool();
			Players.Remove(pLeaver);

			if(Players.Count <= 1)
			{
				Debug.Log("Ending game, no other player!");
				game.GameEnd.EndGame();
			}
		}		
	}

	protected override void OnMainControllerActivated()
	{
		//single player => spawn here
		//MP => spawn in OnRemotePlayerLoadedScene
		if(!isMultiplayer)
		{
			//Debug.Log("Spawn players SP");
			game.Map.SetOnActivated(() =>
					SpawnPlayers(brainiacs.GameInitInfo.Players));

			//test delayed add players
			//game.Map.SetOnActivated(() =>
			//		DoInTime(() => SpawnPlayers(brainiacs.GameInitInfo.Players), 2));
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

	private void SpawnPlayers(List<PlayerInitInfo> pPlayersInfo)
	{
		if(ArePlayersSpawned)
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

			if(debug.LocalImage && spawnedPlayer.LocalImage == null)
			{
				spawnedPlayer.LocalImage = SpawnPlayer(playerInfo, true);
				spawnedPlayer.LocalImage.LocalImageOwner = spawnedPlayer;
				AddPlayer(spawnedPlayer.LocalImage);
			}

			AddPlayer(spawnedPlayer);
			spawnedPlayer.LocalImage?.OnReceivedInitInfo(playerInfo, true);
		}

		//playerSorter.SetPlayers(Players);

		//invoke on activated
		Activate();
		ArePlayersSpawned = true;
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

		bool debug_spawn = false;
		if(debug_spawn)
		{
			if(pPlayerInfo.GetName() == PlayerNameManager.GetPlayerName(1))
				spawnPosition = new Vector2(-3.3f, 3);
			//spawnPosition = Vector2.down;

			if(pPlayerInfo.GetName() == PlayerNameManager.GetPlayerName(2))
				spawnPosition = new Vector2(-1.8f, 2.3f);
			//spawnPosition = Vector2.zero;

			if(pPlayerInfo.GetName() == PlayerNameManager.GetPlayerName(3))
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
		//Debug.Log($"AddPlayer {pPlayer}");

		if(pPlayer.ai.IsTmp)
		{
			//Debug.Log("Skip tmp ai");
			//tmp AIs (Tesla clone) are not registered as active players
			return;
		}

		if(Players.Contains(pPlayer))
		{
			//not bug
			//called from OnReceivedInitInfo - has to be called on every
			//side => just prevent adding the same player
			//Debug.LogError($"Player {pPlayer} already added");
			return;
		}

		Players.Add(pPlayer);
		Debug.Log("Add player " + pPlayer);
		int playersCount = Players.FindAll(a => !a.IsLocalImage).Count;
		Debug.Log($"- {playersCount}/{Players.Count}");

		if(playersCount == brainiacs.GameInitInfo.Players.Count)
		{
			if(pPlayer.IsLocalImage)
				return;

			if(AreAllPlayersAdded)
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
			AreAllPlayersAdded = true;
		}
	}

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
		if(isMultiplayer && !PhotonNetwork.IsMasterClient)
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
		List<Player> otherPlayers = Players.Where(
			//cant use Equals here because of Tesla clone (same number but different type)
			a => a.InitInfo.Number != pOtherThan.InitInfo.Number).ToList();

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
