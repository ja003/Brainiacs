using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : GameController
{
	public List<Player> Players { get; private set; }

	[SerializeField]
	private Player playerPrefab;

	[SerializeField]
	private PlayerSorter playerSorter;

	private void SpawnPlayers(List<PlayerInitInfo> pPlayersInfo)
	{
		Players = new List<Player>();

		foreach(PlayerInitInfo playerInfo in pPlayersInfo)
		{
			Player playerInstance = Instantiate(playerPrefab,
				transform);
			playerInstance.gameObject.name = "Player_" + playerInfo.Name;

			Vector3 spawnPosition = game.MapController.ActiveMap.GetSpawnPoint().position;

			if(playerInfo.Name == "Téra")
				spawnPosition = Vector3.zero;
			if(playerInfo.Name == "Adam")
				spawnPosition = Vector3.left;

			playerInstance.SetInfo(playerInfo, spawnPosition);

			Players.Add(playerInstance);
		}

		playerSorter.SetPlayers(Players);

		//invoke on activated
		Activate();
	}

	protected override void OnGameAwaken()
	{
	}

	protected override void OnGameActivated()
	{
		//Debug.Log($"{gameObject.name} OnGameActivated");
		SpawnPlayers(brainiacs.GameInitInfo.players);
	}
}
