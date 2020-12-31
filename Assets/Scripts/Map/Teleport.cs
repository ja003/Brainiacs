using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MapObstackle
{
	[SerializeField] Teleport linkedTeleport;
	[SerializeField] Transform OutPosition;
	[SerializeField] public EDirection OutDirection;

	protected override void OnCollisionEffect(int pDamage, GameObject pOrigin)
	{
		//Debug.Log("OnCollisionEffect " + pOrigin.name);
		base.OnCollisionEffect(pDamage, pOrigin);
		//Debug.Log("todo: teleport projectile");
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);
		ITeleportable teleportableObject = collision.GetComponent<ITeleportable>();
		if(teleportableObject != null)
		{
			if(teleportedObjects.Contains(teleportableObject))
			{
				//Debug.Log("This object was teleported here recently");
				return;
			}

			ITeleportable teleportedObject = teleportableObject.TeleportTo(linkedTeleport);
			if(teleportedObject != null)
				linkedTeleport.OnTeleported(teleportedObject);
		}
	}

	public Vector2 GetOutPosition()
	{
		return OutPosition ? OutPosition.position : transform.position;
	}

	protected override bool? OnCollision2(int pDamage, Player pOwner, GameObject pOrigin, Vector2 pPush)
	{
		//if teleport collides with teleportable object, dont destroy it.
		//it will be handled in TeleportTo method of individual objects
		ITeleportable teleportableObject = pOrigin.GetComponent<ITeleportable>();
		if(teleportableObject != null)
			return false;

		return base.OnCollision2(pDamage, pOwner, pOrigin, pPush);
	}

	List<ITeleportable> teleportedObjects = new List<ITeleportable>();
	//List<GameObject> teleportedObjects = new List<GameObject>();
	private void OnTeleported(ITeleportable pObject)
	{
		teleportedObjects.Add(pObject);
		DoInTime(() => teleportedObjects.Remove(pObject), 1);
	}

	
}

public interface ITeleportable
{
	/// <summary>
	/// Returns the teleported object.
	/// Eg. Player returns the same instance, Projectile returns the new instance
	/// of the projectile (easier than sending photon message to sync teleporting)
	/// </summary>
	ITeleportable TeleportTo(Teleport pTeleport);
}