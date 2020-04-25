using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiEvade : AiGoalController
{
    public AiEvade(PlayerAiBrain pBrain, Player pPlayer) : base(pBrain, pPlayer)
    {
    }



    public override void Evaluate()
    {
        List<Projectile> activeProjectiles = game.ProjectileManager.ActiveProjectiles;

        //sort ascending based on distance to me -> evaluate the colsest first
        activeProjectiles.Sort((a, b) => 
            Vector3.Distance(playerPosition, a.transform.position).CompareTo(
                Vector3.Distance(playerPosition, b.transform.position)));

        isProjectileGoingToHitMe = false;
        foreach(var projectile in activeProjectiles)
        {
            //ignore my projectiles
            if(projectile.Owner.Equals(player))
            {
                continue;
            }
            isProjectileGoingToHitMe = IsGoingToHitMe(projectile);
            if(isProjectileGoingToHitMe)
            {
                //pick good evasion move taget - perpendicular to projectile direction
                Vector2 perpDir = Vector2.Perpendicular(new Vector2(projectile.Direction.x, projectile.Direction.y));
                //it can be 1 way or the opposite - pick randomly
                if(Utils.TossCoin())
                    perpDir *= 1;
                
                //if there is any map object, try the other direction
                if(Physics2D.Raycast(playerPosition2D, perpDir, 1, game.Layers.MapObject))
                    perpDir *= -1;

                Vector3 perpDir3 = new Vector3(perpDir.x, perpDir.y).normalized;
                evadeMoveTarget = playerPosition + perpDir3;

                Utils.DebugDrawCross(perpDir3, Color.cyan);
            }
        }
    }

    bool isProjectileGoingToHitMe;
    Vector3 evadeMoveTarget;

    /// <summary>
    /// Check if projectile might possibly hit me
    /// </summary>
    private bool IsGoingToHitMe(Projectile pProjectile)
    {
        Vector2 dirProjToMe = playerPosition - pProjectile.transform.position;
        float angle = Vector2.Angle(dirProjToMe, pProjectile.Direction);
        //angle (me, proj, projDir) is too high
        const int min_hit_angle = 20;
        if(angle > min_hit_angle)
        {
            //Debug.Log("Projectile is not endagering me. angle = " + angle);
            return false;

        }

        Vector3 crossProduct = Vector3.Cross(pProjectile.Direction, playerPosition - pProjectile.transform.position);

        //distance to closest point to player in projectile direction
        float distToImpact = crossProduct.magnitude;
        //Debug.Log($"Dist to projectile is {distToImpact:0.00} | cross = {crossProduct}");

        //if higher than body size => shouldnt hit me
        return distToImpact < PlayerVisual.PlayerBodySize / 2;
    }


    public override int GetPriority()
    {
        return isProjectileGoingToHitMe ? 10 : 0;
    }

    public override Vector3 GetTarget()
    {
        return isProjectileGoingToHitMe ? evadeMoveTarget : playerPosition;
    }
}
