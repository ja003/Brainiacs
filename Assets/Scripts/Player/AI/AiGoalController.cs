using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiGoalController : AiController
{
    public AiGoalController(PlayerAiBrain pBrain, Player pPlayer) : base(pBrain, pPlayer)
    {
    }

    /// <summary>
    /// 0 - 10
    /// </summary>
    public abstract int GetPriority();

    public abstract Vector3 GetTarget();

    public abstract void Evaluate();
}
