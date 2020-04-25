using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiController
{
    protected Player player;
    protected PlayerAiBrain brain;

    protected Game game => Game.Instance;
    protected Brainiacs brainiacs => Brainiacs.Instance;

    protected Vector3 playerPosition => player.transform.position;
    protected Vector2 playerPosition2D => player.transform.position;

    public AiController(PlayerAiBrain pBrain, Player pPlayer)
    {
        player = pPlayer;
        brain = pBrain;
    }

    
}
