using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstacle : MapObject
{
	private void OnEnable()
	{
		game.Map.RegisterObstacle(this);
	}
	private void OnDisable()
	{
		//we have to do unregister because of debug - when some map is already present in scene
		if(Game.IsInstantiated)
			game.Map.UnregisterObstacle(this);
	}

	protected override void OnDestroyed()
	{
		//todo: or should it be destroyable?
		//if so => play destroy animation + implement health states visual
		Debug.LogError("Obstacle shouldnt be destroyed");
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
		brainiacs.AudioManager.PlaySound(ESound.Map_Obstacle_Hit, audioSource);
		if(!pIsRPC)
			game.Map.Photon.Send(EPhotonMsg.Map_Obstacle_DoCollisionEffect, name.GetHashCode());
	}

}
