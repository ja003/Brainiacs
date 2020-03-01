using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileCollisionHandler 
{
	//TRUE = destroy projectile
	bool OnCollision(Projectile pProjectile);
}
