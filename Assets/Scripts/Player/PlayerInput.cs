using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public PlayerKeys Keys;


	[SerializeField]
	private PlayerMovement movement;

	[SerializeField]
	private PlayerWeaponController weapon;

	void FixedUpdate()
	{
		ProcessMovementInput();

		ProcessActionInput();

		//item use..
		
	}
	
	private void ProcessMovementInput()
	{
		bool movementRequested = false;
		foreach(EDirection dir in Enum.GetValues(typeof(EDirection)))
		{
			if(movementRequested = IsMovementRequested(dir))
			{
				movement.Move(dir);
				break;
			}
		}
		if(!movementRequested)
			movement.Idle();

		//HACK
		movement.SetSpeedMultiplier(Input.GetKey(KeyCode.LeftShift) ? 10 : 1);

	}

	private void ProcessActionInput()
	{
		if(Input.GetKeyDown(Keys.swapWeapon))
			weapon.SwapWeapon();

		if(Input.GetKeyDown(Keys.useWeapon))
			weapon.UseWeapon();

	}

	private bool IsMovementRequested(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return Input.GetKey(Keys.moveUp); //todo: movement wheel on mobile
			case EDirection.Right:
				return Input.GetKey(Keys.moveRight);
			case EDirection.Down:
				return Input.GetKey(Keys.moveDown);
			case EDirection.Left:
				return Input.GetKey(Keys.moveLeft);
		}
		return false;
	}
}

public enum EDirection
{
	None,
	Up,
	Right,
	Down,
	Left
}
