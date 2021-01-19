using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialNobelMinePhoton : PlayerWeaponSpecialPrefabPhoton
{
	[SerializeField] SpecialNobelMine mine = null;

	protected override bool CanSend3(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			case EPhotonMsg.Special_Nobel_Spawn:
			case EPhotonMsg.Special_Nobel_Explode:
				return IsMine;
		}

		Debug.LogError("Cant send another message");
		return false;
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
			case EPhotonMsg.Special_Nobel_Explode:
				int sortOrder = (int)pParams[0];
				mine.Explode(sortOrder, true);
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
