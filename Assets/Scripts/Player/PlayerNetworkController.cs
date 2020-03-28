using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class PlayerNetworkController : PhotonMessenger
{
	public PhotonPlayer photonPlayer;

	Player player;
	PlayerInitInfo playerInfo;

	bool isItMe => player.IsItMe;
	bool inited;

	protected override void Awake()
	{
		//has to be assigned for remote players
		player = GetComponent<Player>();
		base.Awake();
	}

	internal void Init(PlayerInitInfo pInfo)
	{
		playerInfo = pInfo;
		photonPlayer = playerInfo.PhotonPlayer;
		inited = true;

		Debug.Log($"{this} IsMine: {view.IsMine}, isItMe: {isItMe}");

		if(PhotonNetwork.IsMasterClient)
		{
			Debug.Log(this + " send init Info");

			//DoInTime(() => Send(
			//	EPhotonMsg.Player_InitPlayer, pPlayerInfo.Number), 1);
			Send(EPhotonMsg.Player_InitPlayer, playerInfo.Number);
		}

		if(!isItMe && photonPlayer != null)
		{
			view.TransferOwnership(photonPlayer);
			Debug.Log("Transfer ownership to " + photonPlayer.NickName);
		}
	}

	public void debug_SendInitInfo()
	{
		if(PhotonNetwork.IsMasterClient)
			Send(EPhotonMsg.Player_InitPlayer, playerInfo.Number);

	}

	protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		if(!inited && pReceivedMsg != EPhotonMsg.Player_InitPlayer)
		{
			Debug.Log("not inited yet");
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
				player.OnReceivedInitInfo(info);
				break;

			case EPhotonMsg.Player_ShowWeapon:
				EWeaponId weapon = (EWeaponId)pParams[0];
				player.Visual.ShowWeapon(weapon);
				break;
			case EPhotonMsg.Player_HitByProjectile:
				int damage = (int)pParams[0];
				player.Health.HitByProjectile(damage);
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


			case EPhotonMsg.Player_UI_Scoreboard_SetStats:
				int kills = (int)pParams[0];
				int deaths = (int)pParams[1];
				player.Visual.Scoreboard.SetStats(kills, deaths);
				break;


			default:
				Debug.LogError("Message not handled");
				break;
		}
	}

	protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
		if(player.LocalRemote)
		{
			player.LocalRemote.Network.HandleMsg(pMsgType, pParams);
		}
	}

	protected override bool CanSend()
	{
		return isItMe;
	}
}

public enum EPhotonMsg_Player
{
	None = 0,
	ChangeDirection,
	InitPlayer,
	ShowWeapon,
	HitByProjectile,

	UI_PlayerInfo_SetHealth,
	UI_PlayerInfo_SetReloading,
	UI_PlayerInfo_SetAmmo,
	UI_PlayerInfo_SetActiveWeapon,

	UI_Scoreboard_SetStats,
}
