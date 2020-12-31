using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItemPhoton : PoolObjectPhoton
{
	[SerializeField] MapItem item = null;

	public override void OnReturnToPool()
	{
		//throw new System.NotImplementedException();
	}

	protected override bool CanSend2(EPhotonMsg pMsgType)
	{
		switch(pMsgType)
		{
			//master initiates
			case EPhotonMsg.MapItem_Init:
				return IsMine;

			//explosion can be started on both sides
			case EPhotonMsg.MapItem_DoExplosionEffect:
				return true;

				//only master can return items to pool (master owns all map items)
				//case EPhotonMsg.MapItem_ReturnToPool:
				//    return !view.IsMine;
		}

		return false;
	}

	protected override void HandleMsg2(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.MapItem_Init:
				Vector2 pos = (Vector2)pParams[0];
				MapItem.EType type = (MapItem.EType)pParams[1];
				int subtypeIndex = (int)pParams[2];
				item.Init(pos, type, subtypeIndex);
				break;

			case EPhotonMsg.MapItem_DoExplosionEffect:
				item.DoExplosionEffect(true);
				break;

			//case EPhotonMsg.MapItem_ReturnToPool:
			//    item.ReturnToPool();
			//    break;

			default:
				OnMsgUnhandled(pReceivedMsg);
				break;
		}
	}

	protected override void debug_SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
	}
}
