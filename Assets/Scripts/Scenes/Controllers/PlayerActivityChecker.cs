using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Active only during multiplayer game.
/// Periodically checks activity of other players and sends activity signal to others.
/// If some player is inactive for too long he will be put out of the playground.
/// 
/// Main reason: when user on Android drops the game to background it becomes inactive
/// and it is not possible to let others know.
/// </summary>
public class PlayerActivityChecker : GameController
{
	//records of times of last received signal from players
	Dictionary<Player, float> lastPlayersActivity = new Dictionary<Player, float>();

	[SerializeField] bool debug_instaKill;
	[SerializeField] bool debug_shortInactivity;
	[SerializeField] public bool debug_includeLocalPlayers;

	//visualize bad connection after inactivity of this length
	private const float SHORT_INACTIVITY = .5f;
	//hide player object after inactivity of this length
	private const float LONG_INACTIVITY = 3f;

	protected override void OnMainControllerAwaken()
	{
	}

	public void Init(List<Player> pPlayers)
	{
		if(!brainiacs.GameInitInfo.IsMultiplayer() && !debug_includeLocalPlayers)
			return;

		foreach(Player player in pPlayers)
		{
			if(!debug_includeLocalPlayers && player.InitInfo.PhotonPlayer.IsLocal)
				continue;

			lastPlayersActivity.Add(player, 0);
		}

		DoInTime(SendActivitySignal, 1);
		DoInTime(CheckOtherPlayersActivity, 2);
	}

	private void SendActivitySignal()
	{
		game.Photon.Send(EPhotonMsg.Game_PlayerActivitySignal, PhotonNetwork.LocalPlayer.ActorNumber);
		DoInTime(SendActivitySignal, 0.5f);
	}

	private void CheckOtherPlayersActivity()
	{
		foreach(var playerActivity in lastPlayersActivity)
		{
			float inactivityDuration = Time.time - playerActivity.Value;
			bool isInactiveShortly = IsInactiveShortly(playerActivity.Key);
			bool isInactiveLong = inactivityDuration > LONG_INACTIVITY;

			if(PhotonNetwork.IsMasterClient)
				Debug.Log($"Player {playerActivity.Key} inactive for {inactivityDuration} seconds.");

			if(!isInactiveShortly)
			{
				playerActivity.Key.Visual.SetVisible(true);
				playerActivity.Key.Collider.enabled = true;
				continue;
			}
			if(isInactiveLong)
			{
				Debug.Log($"HIDE inactive player {playerActivity.Key}");
				playerActivity.Key.Collider.enabled = false;
				playerActivity.Key.Visual.SetVisible(false);
				continue;
			}

			//isInactiveShortly => fade animation to visualize inactivity.
			//bad connection icon is handle in UIPlayerInfoElement.
			playerActivity.Key.Visual.Blink();
		}

		DoInTime(CheckOtherPlayersActivity, 0.5f);
	}

	public bool IsInactiveShortly(Player pPlayer)
	{
		if(debug_shortInactivity)
			return true;

		float lastActiveTime;
		if(!lastPlayersActivity.TryGetValue(pPlayer, out lastActiveTime))
		{
			Debug.LogError($"Player {pPlayer} not found");
			return false;
		}
		float inactivityDuration = Time.time - lastActiveTime;

		return inactivityDuration > SHORT_INACTIVITY;
	}

	internal void OnReceiveSignal(Photon.Realtime.Player pSender)
	{
		List<Player> playersMatch = new List<Player>();

		foreach(var player in lastPlayersActivity.Keys)
		{
			//there can be more players with same PhotonPlayer (on 1 PC)
			if(player.InitInfo.PhotonPlayer == pSender)
			{
				playersMatch.Add(player);
				if(PhotonNetwork.IsMasterClient)
					Debug.Log($"Received singnal from {player} at {Time.time}");
			}
		}

		foreach(Player player in playersMatch)
		{
			lastPlayersActivity[player] = Time.time;
		}
	}


	private void OnApplicationFocus(bool focus)
	{
		//to prevent insta death in editor
		if(Time.time < 2)
			return;

		if(!PlatformManager.IsMobile() && !debug_instaKill)
			return;

		if(debug_instaKill)
			Debug.Log("debug_instaKill");

		OnApplicationActive(focus);
	}

	/// <summary>
	/// When user activates the app on phone, it means that he was alt-tabbed and wasn't
	/// active in multiplayer game.
	/// He gets punished => instakill + warning.
	/// </summary>
	private void OnApplicationActive(bool pActive)
	{
		//player gets instakilled only when activating the app
		//no reason to do it when he deactivates it because he wouldnt see it anyway
		bool shouldBeInstaKilled = pActive && isMultiplayer && PlatformManager.IsMobile();
		Debug.Log("shouldBeInstaKilled " + shouldBeInstaKilled);

		if(!shouldBeInstaKilled && !debug_instaKill)
			return;

		Player myPlayer = game.PlayerManager.GetPlayer(PhotonNetwork.LocalPlayer);
#if UNITY_EDITOR
		if(debug_instaKill)
			myPlayer = game.PlayerManager.GetPlayer(1);
#endif

		//respawn handled in Health::OnDeadAnimFinished
		myPlayer.Health.InstaKill();
	}
}
