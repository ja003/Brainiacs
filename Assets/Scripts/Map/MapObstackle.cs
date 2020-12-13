using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstackle : MapObject
{
	private void OnEnable()
	{
		game.Map.RegisterObstackle(this);
	}
	private void OnDisable()
	{
		//we have to do unregister because of debug - when some map is already present in scene
		if(Game.IsInstantiated)
			game.Map.UnregisterObstackle(this);
	}

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
		brainiacs.AudioManager.PlaySound(ESound.Map_Obstackle_Hit, audioSource);
		if(!pIsRPC)
			game.Map.Photon.Send(EPhotonMsg.Map_Obstackle_DoCollisionEffect, name.GetHashCode());
	}

}
