using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPhoton : PhotonMessenger
{
	Game game => Game.Instance;

	protected override bool CanSend(EPhotonMsg pMsgType)
	{
		if(pMsgType == EPhotonMsg.Map_Obstacle_DoCollisionEffect)
			return true;

		return false;
	}

	protected override void HandleMsg(EPhotonMsg pReceivedMsg, object[] pParams, ByteBuffer bb)
	{
		switch(pReceivedMsg)
		{
			case EPhotonMsg.Map_Obstacle_DoCollisionEffect:
				int obstacleId = (int)pParams[0];
				MapObstacle obstacle = game.Map.GetObstacle(obstacleId);
				obstacle.DoCollisionEffect(true);
				break;

			default:
				Debug.LogError("Message not handled");
				break;
		}
	}

	protected override void debug_SendNotMP(EPhotonMsg pMsgType, object[] pParams)
	{
	}
}
