﻿using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class ProjectilePhoton : PhotonMessenger
{
	private Projectile projectile;

	protected override void Awake()
	{
		projectile = GetComponent<Projectile>();
		base.Awake();
	}

	protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Projectile_Spawn:
				Vector3 projectileDirection = (Vector3)pParams[0];
				EWeaponId weapon = (EWeaponId)pParams[1];
				EDirection playerDirection = (EDirection)pParams[2];
				projectile.SetSpawn(projectileDirection, weapon, playerDirection);

				break;
			default:
				Debug.LogError("Message not handled");
				break;
		}
	}

	protected override void SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
		if(projectile.LocalImage)
		{
			projectile.LocalImage.Photon.HandleMsg(pMsgType, pParams);
		}
	}

	internal void Destroy()
	{
		PhotonNetwork.Destroy(view);
		projectile.LocalImage?.Photon.Destroy();
	}

	protected override bool CanSend(EPhotonMsg pMsgType)
	{
		return view.IsMine || projectile.LocalImage;
	}
}