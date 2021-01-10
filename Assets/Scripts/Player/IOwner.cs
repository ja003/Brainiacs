using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides access to a player owning this object.
/// - Player => the same object
/// - DaVinci tank => owner
/// - Projectile => todo: not needed now but might be used in some situation?
/// </summary>
public interface IOwner
{
	Player GetOwner();
}
