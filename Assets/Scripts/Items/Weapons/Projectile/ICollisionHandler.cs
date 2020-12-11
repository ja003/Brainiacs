using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: rename dmaage handler?
/// </summary>
public interface ICollisionHandler 
{
	//TRUE = destroy projectile
	//bool OnCollision(Projectile pProjectile);

	bool OnCollision(int pDamage, Player pOwner, GameObject pOrigin, Vector2 pPush);
}
