using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public PlayerKeys Keys;


	[SerializeField]
	private PlayerMovement movement;

	void FixedUpdate()
	{
		ProcessMovementInput();

		//item use..
		
	}
	
	private void ProcessMovementInput()
	{
		foreach(EDirection dir in Enum.GetValues(typeof(EDirection)))
		{
			if(IsMovementRequested(dir))
			{
				movement.Move(dir);
				break;
			}
		}

		//HACK
		movement.SetSpeedMultiplier(Input.GetKey(KeyCode.LeftShift) ? 10 : 1);

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
