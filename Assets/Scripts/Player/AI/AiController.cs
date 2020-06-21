using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiController
{
    protected Player player;
    protected PlayerAiBrain brain;

    protected Game game => Game.Instance;
    protected Brainiacs brainiacs => Brainiacs.Instance;

    protected IPathFinder pathFinder => game.Map.ActiveMap.PathFinder;

    //protected Vector2 playerPosition => player.transform.position; //NO!
    protected Vector2 playerPosition => player.Position;
    protected Vector3 playerPosition3D => player.Position3D;

    public AiController(PlayerAiBrain pBrain, Player pPlayer)
    {
        player = pPlayer;
        brain = pBrain;
    }

    
}
