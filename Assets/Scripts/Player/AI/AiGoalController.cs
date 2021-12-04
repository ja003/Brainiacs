using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiGoalController : AiController
{
    EAiGoal goal;

    public AiGoalController(Player pPlayer, EAiGoal pGoal) : base(pPlayer)
    {
        goal = pGoal;
    }

    /// <summary>
    /// 0 - 10
    /// </summary>
    public abstract int GetPriority();

    public abstract Vector2 GetTarget();

    public abstract void Evaluate();

    public override string ToString()
    {
        return $"Goal controller: {goal}";
    }
}
