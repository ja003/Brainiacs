using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEffectManager : GameBehaviour
{
	internal void HandleEffect(EGameEffect pType)
	{
		//game effects are handled only on master side
		if(!brainiacs.PhotonManager.IsMaster())
		{
			game.Photon.Send(EPhotonMsg.Game_HandleEffect, pType);
			return;
		}

		switch(pType)
		{
			case EGameEffect.None:
				break;
			case EGameEffect.Night:
				game.Lighting.SetNight(5);
				break;
			default:
				break;
		}
	}
}
