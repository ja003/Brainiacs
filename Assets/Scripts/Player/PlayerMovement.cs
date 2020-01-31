﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private Player player;

	[SerializeField]
	private PlayerWeaponController weapon;

	private const float movementSpeed = 0.05f;

	private float movementSpeedMultiplier = 1;

	EDirection currentDirection;

	internal void Move(EDirection pDirection)
	{
		if(currentDirection != pDirection)
		{
			weapon.OnChangeDirection(pDirection);
		}
		currentDirection = pDirection;
		transform.position += GetVector(pDirection) * movementSpeed * movementSpeedMultiplier;
	}

	private Vector3 GetVector(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return Vector3.up;
			case EDirection.Right:
				return Vector3.right;
			case EDirection.Down:
				return Vector3.down;
			case EDirection.Left:
				return Vector3.left;
		}
		return Vector3.zero;
	}

	internal void SetSpeedMultiplier(float pValue)
	{
		movementSpeedMultiplier = pValue;
	}
}
