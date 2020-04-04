using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : BrainiacsBehaviour
{
	[SerializeField]
	private Player player;

	[SerializeField]
	public Collider2D PlayerCollider;

	//[SerializeField]
	//private PlayerWeaponController weapon;

	private const float MOVE_SPEED_BASE = 0.05f;


	public EDirection CurrentDirection { get; private set; } = EDirection.Right;

	internal void SpawnAt(Vector3 pPosition)
	{
		transform.position = pPosition;
		//Move(EDirection.Right); //set init direction
		player.Visual.OnSpawn();
	}

	public void Move(EDirection pDirection)
	{
		//Debug.Log(gameObject.name + " move " + pDirection);
		if(CurrentDirection != pDirection)
		{
			CurrentDirection = pDirection;
			player.Visual.OnDirectionChange(pDirection);
			player.WeaponController.OnDirectionChange(pDirection);
		}
		CurrentDirection = pDirection;
		transform.position += Utils.GetVector3(pDirection) *
			MOVE_SPEED_BASE * player.Stats.Speed;
		player.Visual.Move();

		player.LocalRemote?.Movement.Move(pDirection);
	}

	public void Idle()
	{
		//Debug.Log(gameObject.name + " idle");
		player.Visual.Idle();
	}
}
