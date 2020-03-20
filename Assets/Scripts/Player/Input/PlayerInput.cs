using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : GameBehaviour
{
	PlayerKeys keys;

	[SerializeField]
	private Player player;

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

	public void Init(PlayerInitInfo pPlayerInfo)
	{
		if(pPlayerInfo.PlayerType != EPlayerType.LocalPlayer)
		{
			//todo: send message to remote player
			return;
		}
		if(PlatformManager.GetPlatform() == EPlatform.PC)
		{
			keys = brainiacs.PlayerKeysManager.GetPlayerKeys(pPlayerInfo.Number);
		}
		else
		{
			game.MobileInput.btnShoot.OnPressedAction =
				() => weapon.UseWeapon();
			game.MobileInput.btnShoot.OnPointerUpAction =
				() => weapon.StopUseWeapon();

			game.MobileInput.btnSwap.OnPointerDownAction =
				() => weapon.SwapWeapon();

			game.MobileInput.moveJoystick.OnUpdateDirection += HandleMoveJoystick;
		}

	}

	private void HandleMoveJoystick(Vector2 pInput)
	{
		const float move_threshold = 0.5f;

		EDirection direction = EDirection.None;

		//if both direction (X,Y) are requested, handle
		//the one with higher value
		if(Math.Abs(pInput.y) > Math.Abs(pInput.x))
		{
			if(pInput.y > move_threshold)
				direction = EDirection.Up;
			else if(pInput.y < -move_threshold)
				direction = EDirection.Down;
		}
		else
		{
			if(pInput.x > move_threshold)
				direction = EDirection.Right;
			else if(pInput.x < -move_threshold)
				direction = EDirection.Left;
		}

		if(direction == EDirection.None)
			return;

		movement.Move(direction);
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
		if(Input.GetKeyDown(KeyCode.Backslash))
		{
			player.Stats.SetSpeed(10, 1);
		}
	}


	private void ProcessActionInput()
	{
		if(Input.GetKeyDown(keys.swapWeapon))
			weapon.SwapWeapon();

		if(Input.GetKey(keys.useWeapon))
			weapon.UseWeapon();

		if(Input.GetKeyUp(keys.useWeapon))
			weapon.StopUseWeapon();


	}

	private bool IsMovementRequested(EDirection pDirection)
	{
		switch(pDirection)
		{
			case EDirection.Up:
				return Input.GetKey(keys.moveUp); //todo: movement wheel on mobile
			case EDirection.Right:
				return Input.GetKey(keys.moveRight);
			case EDirection.Down:
				return Input.GetKey(keys.moveDown);
			case EDirection.Left:
				return Input.GetKey(keys.moveLeft);
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
