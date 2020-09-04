using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstackle : MapObject
{
	//protected override void Awake()
	//{
	//	game.Map.RegisterObstackle(this);
	//	base.Awake();
	//}

	protected override void OnSetActive0(bool pValue)
	{
		spriteRend.enabled = pValue;
		boxCollider2D.enabled = pValue;

		if(pValue)
			game.Map.RegisterObstackle(this);
		else //we have to do unregister because of debug - when some map is already present in scene
			game.Map.UnregisterObstackle(this);

	}


	//protected override bool CanSendMsg(EPhotonMsg pMsgType)
	//{
	//	throw new System.NotImplementedException();
	//}

	protected override void OnCollisionEffect(int pDamage, GameObject pOrigin)
	{
		//Debug.Log("todo: play collision sound, make effect...");
		if(pDamage <= 0)
			return;
		DoCollisionEffect(false);
	}

	/// <summary>
	/// Sound.
	/// RPC - called from anywhere
	/// pIsRPC = Is called from RPC => dont send another message.
	/// </summary>
	public void DoCollisionEffect(bool pIsRPC)
	{
		SoundController.PlaySound(ESound.Map_Obstackle_Hit, audioSource);
		if(!pIsRPC)
			game.Map.Photon.Send(EPhotonMsg.Map_Obstackle_DoCollisionEffect, name.GetHashCode());
	}

}
