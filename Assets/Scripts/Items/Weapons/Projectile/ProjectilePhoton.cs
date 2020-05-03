using FlatBuffers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

public class ProjectilePhoton : PoolObjectPhoton
{
	private Projectile projectile;

	protected override void Awake()
	{
		projectile = GetComponent<Projectile>();
		base.Awake();
	}

	protected override void HandleMsg2(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
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
				Debug.LogError(this.gameObject.name + " message not handled " + pReceivedMsg);
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

	//internal void Destroy()
	//{
	//	PhotonNetwork.Destroy(view);
	//	projectile.LocalImage?.Photon.Destroy();
	//}

	protected override bool CanSendMsg(EPhotonMsg pMsgType)
	{
		return view.IsMine || projectile.LocalImage;
	}
}
