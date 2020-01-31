using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersController : GameController
{
	public List<Player> Players { get; private set; }

	[SerializeField]
	private Player playerPrefab;

	private void SpawnPlayers(List<PlayerInitInfo> pPlayersInfo)
	{
		Players = new List<Player>();

		foreach(PlayerInitInfo playerInfo in pPlayersInfo)
		{
			Player playerInstance = Instantiate(playerPrefab,
				transform);
			playerInstance.transform.position =
				game.MapController.ActiveMap.GetSpawnPoint().position;

			playerInstance.SetInfo(playerInfo);

			Players.Add(playerInstance);
		}
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
