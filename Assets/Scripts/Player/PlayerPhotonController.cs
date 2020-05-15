using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class PlayerPhotonController : PoolObjectPhoton
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
	bool isInited;

	Game game => Game.Instance;

	protected override bool IsLocalImage()
	{
		return player.IsLocalImage;
	}

	internal void Init2(PlayerInitInfo pInfo)
	{
		if(isInited)
			return;
		playerInfo = pInfo;
		PhotonPlayer = playerInfo.PhotonPlayer;

		//Debug.Log($"{this} IsMine: {IsMine}, isItMe: {isItMe}");

		if(PhotonNetwork.IsMasterClient && !player.IsItMe && PhotonPlayer != null)
		{
			view.TransferOwnership(PhotonPlayer);
			Debug.Log("Transfer ownership to " + PhotonPlayer.NickName);
		}

		//Debug.Log(this + " send init Info");
		//DoInTime(() => Send(
		//	EPhotonMsg.Player_InitPlayer, pPlayerInfo.Number), 1);
		Send(EPhotonMsg.Player_InitPlayer, playerInfo.Number);

		isInited = true;
	}

	public override void OnReturnToPool()
	{
		isInited = false;
		base.OnReturnToPool();
	}

	/// <summary>
	/// Player can send data only if it is mine player and is inited.
	/// There are execeptions.
	/// </summary>
	protected override bool CanSend2(EPhotonMsg pMsgType)
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

	protected override void HandleMsg2(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		if(!isInited && pReceivedMsg != EPhotonMsg.Player_InitPlayer)
		{
			Debug.Log(gameObject.name + " not inited yet. " + pReceivedMsg);
			player.OnPlayerInited.AddAction(() => HandleMsg2(pReceivedMsg, pParams, bb));
			return;
		}

		switch(pReceivedMsg)
		{
			case EPhotonMsg.Player_ChangeDirection:
				EDirection dir = (EDirection)pParams[0];
				player.Visual.OnDirectionChange(dir);
				return;

			case EPhotonMsg.Player_InitPlayer:
				int playerNumber = (int)pParams[0];
				PlayerInitInfo info = brainiacs.GameInitInfo.GetPlayer(playerNumber);
				player.OnReceivedInitInfo(info, false);
				return;

			case EPhotonMsg.Player_ShowWeapon:
				EWeaponId weapon = (EWeaponId)pParams[0];
				player.Visual.ShowWeapon(weapon);
				return;
			case EPhotonMsg.Player_ApplyDamage:
				int damage = (int)pParams[0];
				playerNumber = (int)pParams[1];
				Player originOfDamage = game.PlayerManager.GetPlayer(playerNumber);
				player.Health.ApplyDamage(damage, originOfDamage);
				return;

			case EPhotonMsg.Player_AddKill:
				//EPlayerStats stat = (EPlayerStats)pParams[0];
				//int value = (int)pParams[1];
				//bool force = (bool)pParams[2];
				bool force = (bool)pParams[0];
				player.Stats.AddKill(force);
				return;

			case EPhotonMsg.Player_SetSyncPosition:
				Vector3 pos = (Vector3)pParams[0];
				dir = (EDirection)pParams[1];
				bool isActuallyMoving = (bool)pParams[2];
				float speed = (float)pParams[3];
				bool instantly = (bool)pParams[4];
				player.Movement.SetSyncPosition(pos, dir, isActuallyMoving, speed, instantly);
				return;

			case EPhotonMsg.Player_UI_PlayerInfo_SetHealth:
				int health = (int)pParams[0];
				player.Visual.PlayerInfo.SetHealth(health);
				return;

			case EPhotonMsg.Player_UI_PlayerInfo_SetReloading:
				bool isReloading = (bool)pParams[0];
				float reloadTime = (float)pParams[1];
				player.Visual.PlayerInfo.SetReloading(isReloading, reloadTime);
				return;
			case EPhotonMsg.Player_UI_PlayerInfo_SetAmmo:
				int ammo = (int)pParams[0];
				bool weaponUsed = (bool)pParams[1];
				player.Visual.PlayerInfo.SetAmmo(ammo, weaponUsed);
				return;
			case EPhotonMsg.Player_UI_PlayerInfo_SetActiveWeapon:
				weapon = (EWeaponId)pParams[0];
				float cadency = (float)pParams[1];
				player.Visual.PlayerInfo.SetActiveWeapon(weapon, cadency);
				return;


			case EPhotonMsg.Player_UI_Scoreboard_SetScore:
				int kills = (int)pParams[0];
				int deaths = (int)pParams[1];
				player.Visual.Scoreboard.SetScore(kills, deaths);
				return;

			case EPhotonMsg.Player_Visual_OnDamage:
				player.Visual.OnDamage();
				break;

			case EPhotonMsg.Player_Visual_OnDie:
				player.Visual.OnDie();
				break;

			default:
				OnMsgUnhandled(pReceivedMsg);
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