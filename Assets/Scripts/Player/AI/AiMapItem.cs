using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMapItem : AiGoalController
{
    public AiMapItem(PlayerAiBrain pBrain, Player pPlayer) : base(pBrain, pPlayer)
    {
    }

    bool isMapItemClose;
    Vector3 mapItemMoveTarget;

    public override void Evaluate()
    {
        List<MapItem> activeItems = game.Map.Items.ActiveItems;

        //sort ascending based on distance to me -> evaluate the colsest first
        activeItems.Sort((a, b) =>
            Vector3.Distance(playerPosition, a.transform.position).CompareTo(
                Vector3.Distance(playerPosition, b.transform.position)));

        isMapItemClose = false;
        foreach(var item in activeItems)
        {
            float dist = Vector3.Distance(playerPosition, item.transform.position);
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
        return isMapItemClose ? 5 : 0;
    }

    public override Vector3 GetTarget()
    {
        return isMapItemClose ? mapItemMoveTarget : playerPosition;
    }

}
