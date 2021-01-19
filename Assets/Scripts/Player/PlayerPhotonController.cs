using FlatBuffers;
using Photon.Pun;
using Smooth;
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

	protected override void Awake()
	{
		player.Movement.SmoothSync.enabled = false;
		base.Awake();
	}

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

		//Debug.Log("send Player_InitPlayer " + playerInfo.Number);

		Send(EPhotonMsg.Player_InitPlayer, playerInfo.Number);

		isInited = true;

		//if not disabled from start it throws error on client
		player.Movement.SmoothSync.enabled = true;
	}

	public override void OnReturnToPool()
	{
		isInited = false;
		base.OnReturnToPool();
	}

	public override void Send(EPhotonMsg pMsgType, params object[] pParams)
	{
		//todo: this doesnt work?
		// => handled in PlayerPhoton.HandleMsg2
		if(IsPhotonPlayerMsg(pMsgType))
		{
			//Debug.Log($"send msg {pMsgType} to {PhotonPlayer}, LOCAL = {PhotonPlayer.IsLocal}");
			if(PhotonPlayer.IsLocal)
				return;
			Send(pMsgType, PhotonPlayer, pParams);
		}
		else
		{
			base.Send(pMsgType, pParams);
		}

	}

	private bool IsPhotonPlayerMsg(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			//case EPhotonMsg.Player_ChangeDirection:
			//case EPhotonMsg.Player_InitPlayer:
			//case EPhotonMsg.Player_ShowWeapon:
			case EPhotonMsg.Player_ApplyDamage:
			case EPhotonMsg.Player_AddKill:
			case EPhotonMsg.Player_Push:
				return true;
		}
		return false;
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
			//this has been added in 55 but fucked up the player initialization.
			//but maybe this is how it should be.
			//Problem was in PlayerHealth::ApplyDamage - it expects both damage
			//origin and target to be inited.
			//Should they be both inited? 
			//Maybe we should add message Player_OnPlayerInited informing only
			//that remote player was inited and we can mark him as inited?
			//TODO: test how this works and find out reason why this condition
			//		has been added
			//return brainiacs.PhotonManager.IsMaster();

			case EPhotonMsg.Player_ShowWeapon:
				return true;

			case EPhotonMsg.Player_AddKill:
			case EPhotonMsg.Player_ApplyDamage:
			case EPhotonMsg.Player_Push:
				return player.IsInited;
		}

		return player.IsInitedAndMe;
	}

	protected override void HandleMsg2(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		if(!isInited && pReceivedMsg != EPhotonMsg.Player_InitPlayer)
		{
			//Debug.Log(gameObject.name + " not inited yet. " + pReceivedMsg);
			player.OnPlayerInited.AddAction(() => HandleMsg2(pReceivedMsg, pParams, bb));
			return;
		}

		//if(PhotonPlayer != null)
		//	Debug.Log($"Handle {pReceivedMsg}, {PhotonPlayer}, local = {PhotonPlayer.IsLocal}");

		//msg is meant only for 1 specific player
		if(PhotonPlayer != null 
			&& IsPhotonPlayerMsg(pReceivedMsg) 
			&& !PhotonPlayer.IsLocal)
		{
			Debug.LogError($"Handle {pReceivedMsg}, {PhotonPlayer}, local = {PhotonPlayer.IsLocal}");
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
			case EPhotonMsg.Player_Push:
				Vector2 push = (Vector2)pParams[0];
				player.Push.Push(push);
				return;

			case EPhotonMsg.Player_AddKill:
				//Debug.Log("handle AddKill");

				//EPlayerStats stat = (EPlayerStats)pParams[0];
				//int value = (int)pParams[1];
				//bool force = (bool)pParams[2];
				bool force = (bool)pParams[0];
				player.Stats.AddKill(force);
				return;

			//not used - replaced by SmoothSync
			case EPhotonMsg.Player_SetSyncPosition:
				Vector2 pos = (Vector2)pParams[0];
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

			case EPhotonMsg.Player_UI_SetEffectActive:
				EPlayerEffect effect = (EPlayerEffect)pParams[0];
				bool state = (bool)pParams[1];
				player.Stats.StatsEffect.SetEffectActive(effect, state);
				return;

			case EPhotonMsg.Player_OnReceiveDamageEffect:
				player.Health.OnReceiveDamageEffect();
				break;

			case EPhotonMsg.Player_Visual_OnDie:
				player.Visual.OnDie();
				break;

			case EPhotonMsg.Player_DoEliminateEffect:
				player.Health.DoEliminateEffect();
				break;

			case EPhotonMsg.Player_PlayWeaponUseSound:
				EWeaponId id = (EWeaponId)pParams[0];
				player.WeaponController.PlayWeaponUseSound(id);
				return;

			case EPhotonMsg.Player_DoShieldHitEffect:
				player.Stats.DoShieldHitEffect();
				return;

			default:
				OnMsgUnhandled(pReceivedMsg);
				break;

		}

	}

	protected override void debug_SendNotMP(EPhotonMsg pMsgType, object[] pParams)
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