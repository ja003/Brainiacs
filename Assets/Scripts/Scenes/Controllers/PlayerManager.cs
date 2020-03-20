using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : GameController
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
			SpawnPlayer(playerInfo);
		}

		playerSorter.SetPlayers(Players);

		//invoke on activated
		Activate();
	}

	private void SpawnPlayer(PlayerInitInfo pPlayerInfo)
	{
		//Player playerInstance = Instantiate(playerPrefab, transform);
		Player playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Player>();
		playerInstance.transform.parent = transform;


		playerInstance.gameObject.name = "Player_" + pPlayerInfo.Name;

		Vector3 spawnPosition = game.MapController.ActiveMap.GetSpawnPoint().position;

		if(pPlayerInfo.Name == DebugData.GetPlayerName(1))
			spawnPosition = Vector3.down;

		if(pPlayerInfo.Name == DebugData.GetPlayerName(2))
			spawnPosition = Vector3.zero;

		if(pPlayerInfo.Name == DebugData.GetPlayerName(3))
			spawnPosition = Vector3.down;

		playerInstance.SetInfo(pPlayerInfo, spawnPosition);

		Players.Add(playerInstance);
	}

	protected override void OnMainControllerAwaken()
	{
		//map has to be activated first

		//if(brainiacs.GameInitInfo.IsMultiplayer() && !PhotonNetwork.IsMasterClient)
		//{
		//	Debug.Log("Not MasterClient => dont spawn");
		//	return;
		//}

		game.MapController.SetOnActivated(() =>
				SpawnPlayers(brainiacs.GameInitInfo.Players));

		//SpawnPlayers(brainiacs.GameInitInfo.Players);
	}
}
