﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PlayerBehaviour
{
	[SerializeField] public Collider2D PlayerCollider = null;

	private const float MOVE_SPEED_BASE = 0.05f;

	public EDirection CurrentDirection { get; private set; } = EDirection.Right;
	public bool IsMoving { get; private set; }

	private void FixedUpdate()
	{
		if(!player.IsItMe || !IsMoving)
			return;

		Move(CurrentDirection);
	}

	internal void SpawnAt(Vector3 pPosition)
	{
		transform.position = pPosition;
		//Move(EDirection.Right); //set init direction
		player.Visual.OnSpawn();
		SetMove(CurrentDirection);
	}

	public void SetMove(EDirection pDirection)
	{
		if(pDirection == EDirection.None)
		{
			player.Visual.Idle();
			IsMoving = false;
			return;
		}

		bool isDirectionChanged = CurrentDirection != pDirection;
		CurrentDirection = pDirection; //has to be set before OnDirectionChange call
		IsMoving = true;
		if(isDirectionChanged)
		{
			visual.OnDirectionChange(pDirection);
			weapon.OnDirectionChange(pDirection);
		}

		player.LocalImage?.Movement.SetMove(pDirection);
	}


	private void Move(EDirection pDirection)
	{
		if(pDirection == EDirection.None)
		{
			//player.Visual.Idle();
			return;
		}

		//Debug.Log(gameObject.name + " move " + pDirection);
		//if(CurrentDirection != pDirection)
		//{
		//	CurrentDirection = pDirection;
		//	player.Visual.OnDirectionChange(pDirection);
		//	player.WeaponController.OnDirectionChange(pDirection);
		//}
		//CurrentDirection = pDirection;
		transform.position += Utils.GetVector3(pDirection) *
			MOVE_SPEED_BASE * player.Stats.Speed;
		player.Visual.Move();

		player.LocalImage?.Movement.Move(pDirection);
	}

	//public void Idle()
	//{
	//	//Debug.Log(gameObject.name + " idle");
	//	player.Visual.Idle();
	//}
}
