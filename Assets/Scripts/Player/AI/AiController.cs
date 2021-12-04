using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiController
{
	protected Player player;
	protected PlayerAiBTController aiBT;

	protected Game game => Game.Instance;
	protected Brainiacs brainiacs => Brainiacs.Instance;
	protected CDebug debug => CDebug.Instance;

	protected PathFinderController pathFinder => game.Map.PathFinder;

	//protected Vector2 playerPosition => player.transform.position; //NO!
	protected Vector2 playerPosition => player.ColliderCenter;
	protected Vector3 playerPosition3D => player.ColliderCenter3D;

	public AiController(Player pPlayer)
	{
		player = pPlayer;
		aiBT = player.ai;
	}

	float activeSince;
	protected bool isActive => Time.time > activeSince;

	public void DisableFor(float pSeconds)
	{
		activeSince = Time.time + pSeconds;
	}

}
