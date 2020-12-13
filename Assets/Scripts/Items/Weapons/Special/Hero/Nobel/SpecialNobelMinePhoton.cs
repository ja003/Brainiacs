using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialNobelMinePhoton : PlayerWeaponSpecialPrefabPhoton
{
	//[SerializeField] SpecialNobelMine mine = null;

	protected override bool CanSend3(EPhotonMsg pMsgType)
	{
		if(pMsgType != EPhotonMsg.Special_Nobel_Spawn)
		{
			Debug.LogError("Cant send another message");
			return false;
		}

		return IsMine;
	}

	protected override void HandleMsg3(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Special_Nobel_Spawn:
				int playerNumber = (int)pParams[0];
				Player player = Game.Instance.PlayerManager.GetPlayer(playerNumber);
				//mine.Spawn(player);
				break;
			default:
				OnMsgUnhandled(pReceivedMsg);
				break;
		}
	}

	protected override void debug_SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
		//throw new System.NotImplementedException();
	}
}
