using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMapItem : AiGoalController
{
    public AiMapItem(PlayerAiBrain pBrain, Player pPlayer) : base(pBrain, pPlayer, EAiGoal.PickupItem)
    {
    }

    bool isMapItemClose;
    bool isAnyMapItemOnMap;
    Vector2 mapItemMoveTarget;

    public override void Evaluate()
    {
        List<MapItem> activeItems = game.Map.Items.ActiveItems;
        isAnyMapItemOnMap = activeItems.Count > 0;

        //sort ascending based on distance to me -> evaluate the closest first
        activeItems.Sort((a, b) =>
            Vector2.Distance(playerPosition, a.transform.position).CompareTo(
                Vector2.Distance(playerPosition, b.transform.position)));

        isMapItemClose = false;
        foreach(var item in activeItems)
        {
            float dist = Vector2.Distance(playerPosition, item.transform.position);
            if(dist < 5)
            {
                isMapItemClose = true;
                mapItemMoveTarget = item.transform.position;
                Utils.DebugDrawCross(mapItemMoveTarget, Color.cyan);
                break;
            }
        }


    }

    internal void Update()
    {
    }

    public override int GetPriority()
    {
        if(!isAnyMapItemOnMap) return 0;

        float distToTarget = Vector2.Distance(playerPosition, mapItemMoveTarget);
        const int max_prio = 7;
        const int min_measure_dist = 3;
        const int max_measure_dist = 10;
        const int dist_reduce_coef = 5;
        float coef = max_prio - dist_reduce_coef * (distToTarget - min_measure_dist) / max_measure_dist;
        int prio = (int)Mathf.Clamp(coef, 3, 8);
        return prio;
        //return isMapItemClose ? 6 : 0;
    }

    public override Vector2 GetTarget()
    {
        return mapItemMoveTarget;
    }

}
