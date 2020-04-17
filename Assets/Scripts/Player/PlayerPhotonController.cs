using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class PlayerPhotonController : PhotonMessenger
{
	public PhotonPlayer PhotonPlayer;

	private Player _player;
	protected Player player
	{
		get
		{
			if(_player == null)
				_player = GetComponent<Player>();
			return _player;
		}
	}
	PlayerInitInfo playerInfo;

	//bool isItMe => player.IsItMe;
	bool inited;

	Game game => Game.Instance;

	internal void Init(PlayerInitInfo pInfo)
	{
		if(inited)
			return;
		playerInfo = pInfo;
		PhotonPlayer = playerInfo.PhotonPlayer;

		//Debug.Log($"{this} IsMine: {view.IsMine}, isItMe: {isItMe}");

		if(PhotonNetwork.IsMasterClient && !player.IsItMe && PhotonPlayer != null)
		{
			view.TransferOwnership(PhotonPlayer);
			Debug.Log("Transfer ownership to " + PhotonPlayer.NickName);
		}

		//Debug.Log(this + " send init Info");
		//DoInTime(() => Send(
		//	EPhotonMsg.Player_InitPlayer, pPlayerInfo.Number), 1);
		Send(EPhotonMsg.Player_InitPlayer, playerInfo.Number);

		inited = true;
	}

	/// <summary>
	/// Player can send data only if it is mine player and is inited.
	/// There are execeptions.
	/// </summary>
	protected override bool CanSend(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Player_InitPlayer:
			case EPhotonMsg.Player_ShowWeapon:
				return true;
			case EPhotonMsg.Player_AddKill:
			case EPhotonMsg.Player_ApplyDamage:
				return player.IsInited;
		}

		return player.IsInitedAndMe;
	}

	protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		if(!inited && pReceivedMsg != EPhotonMsg.Player_InitPlayer)
		{
			//Debug.Log("not inited yet. " + pReceivedMsg);
			return;
		}

		switch(pReceivedMsg)
		{
			case EPhotonMsg.Player_ChangeDirection:
				EDirection dir = (EDirection)pParams[0];
				player.Visual.OnDirectionChange(dir);
				break;

			case EPhotonMsg.Player_InitPlayer:
				int playerNumber = (int)pParams[0];
				PlayerInitInfo info = brainiacs.GameInitInfo.GetPlayer(playerNumber);
				player.OnReceivedInitInfo(info, false);
				break;

			case EPhotonMsg.Player_ShowWeapon:
				EWeaponId weapon = (EWeaponId)pParams[0];
				player.Visual.ShowWeapon(weapon);
				break;
			case EPhotonMsg.Player_ApplyDamage:
				int damage = (int)pParams[0];
				playerNumber = (int)pParams[1];
				Player originOfDamage = game.PlayerManager.GetPlayer(playerNumber);
				player.Health.ApplyDamage(damage, originOfDamage);
				break;

			case EPhotonMsg.Player_AddKill:
				//EPlayerStats stat = (EPlayerStats)pParams[0];
				//int value = (int)pParams[1];
				//bool force = (bool)pParams[2];
				bool force = (bool)pParams[0];
				player.Stats.AddKill(force);
				break;


			case EPhotonMsg.Player_UI_PlayerInfo_SetHealth:
				int health = (int)pParams[0];
				player.Visual.PlayerInfo.SetHealth(health);
				break;

			case EPhotonMsg.Player_UI_PlayerInfo_SetReloading:
				bool isReloading = (bool)pParams[0];
				float reloadTime = (float)pParams[1];
				player.Visual.PlayerInfo.SetReloading(isReloading, reloadTime);
				break;
			case EPhotonMsg.Player_UI_PlayerInfo_SetAmmo:
				int ammo = (int)pParams[0];
				player.Visual.PlayerInfo.SetAmmo(ammo);
				break;
			case EPhotonMsg.Player_UI_PlayerInfo_SetActiveWeapon:
				weapon = (EWeaponId)pParams[0];
				float cadency = (float)pParams[1];
				player.Visual.PlayerInfo.SetActiveWeapon(weapon, cadency);
				break;


			case EPhotonMsg.Player_UI_Scoreboard_SetScore:
				int kills = (int)pParams[0];
				int deaths = (int)pParams[1];
				player.Visual.Scoreboard.SetScore(kills, deaths);
				break;


			default:
				Debug.LogError("Message not handled");
				break;
		}
	}

	protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
		if(player.LocalImage)
		{
			player.LocalImage.Photon.HandleMsg(pMsgType, pParams);
		}
		else if(player.LocalImageOwner)
		{
			player.LocalImageOwner.Photon.HandleMsg(pMsgType, pParams);
		}
	}

	public void debug_SendInitInfo()
	{
		if(PhotonNetwork.IsMasterClient)
			Send(EPhotonMsg.Player_InitPlayer, playerInfo.Number);

	}
}