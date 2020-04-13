using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionHandler 
{
	//TRUE = destroy projectile
	//bool OnCollision(Projectile pProjectile);

	bool OnCollision(int pDamage, Player pOrigin);
}
