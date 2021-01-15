﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : PlayerBehaviour
{
	PlayerKeys keys;

	void Update()
	{
		//Ai doesnt have input inited
		if(!isInited)
			return;

		if(!player.IsInitedAndMe)
			return;

		if(player.Health.IsDying)
			return;

		if(!game.GameStarted)
			return;

		if(game.GameEnd.GameEnded)
			return;

		if(game.GameTime.IsPaused)
			return;

		//dont calculate keys input on mobile (performance)
		//in editor we still want it for debug
		if(!PlatformManager.IsMobile() || !debug.release)
			CalculateMoveKeys();

		ProcessMovementInput();

		ProcessActionInput();

		//item use..

	}

	bool isInited;
	public void Init(PlayerInitInfo pPlayerInfo)
	{
		//AIs dont use input
		//assign keys to my players
		if(pPlayerInfo.PlayerType == EPlayerType.AI || !pPlayerInfo.IsItMe())
		{
			//todo: send message to remote player
			return;
		}
		if(PlatformManager.GetPlatform() == EPlatform.PC)// && !debug.MobileInput)
		{
			if(pPlayerInfo.Keyset == EKeyset.None)
			{
				Debug.LogError($"{player} has no keyset [not error when start from S_Game]");
				switch(pPlayerInfo.Number)
				{
					case 1:
						pPlayerInfo.Keyset = EKeyset.KeysetA;
						break;
					case 2:
						pPlayerInfo.Keyset = EKeyset.KeysetB;
						break;
					case 3:
						pPlayerInfo.Keyset = EKeyset.KeysetC;
						break;
					case 4:
						pPlayerInfo.Keyset = EKeyset.KeysetD;
						break;
					default:
						pPlayerInfo.Keyset = EKeyset.KeysetA;
						break;
				}
			}

			keys = brainiacs.PlayerKeysManager.GetPlayerKeys(pPlayerInfo.Keyset);
		}

		if(PlatformManager.IsMobile())
		{
			MobileInput mobileInput = game.MobileInput;
			mobileInput.btnShoot.OnPressedAction = () => weapon.UseWeapon();
			mobileInput.btnShoot.OnPointerUpAction = () => weapon.StopUseWeapon();
			mobileInput.btnSwap.OnPointerDownAction = () => weapon.SwapWeapon();

			mobileInput.moveJoystick.OnUpdateDirection += CalculateMoveJoystick;
		}
		isInited = true;
	}

	EDirection joystickDirection;
	Vector2 joystickInput;

	private void CalculateMoveJoystick(Vector2 pInput)
	{
		joystickInput = pInput;

		const float move_threshold = 0.1f;

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

		//if(direction == EDirection.None)
		//	return;
		joystickDirection = direction;

		//movement.SetMove(direction);
	}

	Vector2 keyDirection;

	/// <summary>
	/// Input from keyboard.
	/// small BUG: if holding 2 opposite keys (<-, ->) one direction
	/// is has bigger impact
	/// </summary>
	private void CalculateMoveKeys()
	{
		const float move_change_speed = .25f;
		bool movementRequested = false;

		float vertical = Input.GetAxis("Vertical");

		if(Input.GetKey(keys.moveUp) || vertical > 0.1f)
		{
			//has to be registered in every case
			movementRequested = true;
			//set to zero if going down for fast direction change
			if(keyDirection.y < 0)
				keyDirection.y = 0;
			keyDirection += Vector2.up * move_change_speed;
		}
		if(Input.GetKey(keys.moveRight))
		{
			if(keyDirection.x < 0)
				keyDirection.x = 0;
			movementRequested = true;
			keyDirection += Vector2.right * move_change_speed;
		}
		if(Input.GetKey(keys.moveDown))
		{
			if(keyDirection.y > 0)
				keyDirection.y = 0;
			movementRequested = true;
			keyDirection += Vector2.down * move_change_speed;
		}
		if(Input.GetKey(keys.moveLeft))
		{
			if(keyDirection.x > 0)
				keyDirection.x = 0;
			movementRequested = true;
			keyDirection += Vector2.left * move_change_speed;
		}

		//Debug.Log($"keyDir = {keyDirection} | {movementRequested}");

		if(keyDirection.magnitude > 1)
			keyDirection.Normalize();

		//stop immediately
		if(!movementRequested)
			keyDirection = Vector2.zero;
	}


	private void ProcessMovementInput()
	{
		// ALL DIRECTION MOVEMENT
		if(brainiacs.PlayerPrefs.AllowMoveAllDir)
		{
			if(joystickDirection != EDirection.None)
				movement.SetMove(joystickInput);
			else
				movement.SetMove(keyDirection);

			return;
		}

		// 2D MOVEMENT

		//Debug.Log("ProcessMovementInput");
		bool movementRequested = false;
		foreach(EDirection dir in Enum.GetValues(typeof(EDirection)))
		{
			if(movementRequested = IsMovementRequested(dir))
			{
				movement.SetMove(dir);
				break;
			}
		}
		if(!movementRequested)
			movement.SetMove(EDirection.None);
	}


	private void ProcessActionInput()
	{
		if(PlatformManager.IsMobile())
			return;

		if(Input.GetKeyDown(keys.swapWeapon))
			weapon.SwapWeapon();

		if(Input.GetKeyUp(keys.useWeapon))
			weapon.StopUseWeapon();

		if(Input.GetKey(keys.useWeapon))
			weapon.UseWeapon();
		//auto detect if weapon is used when shouldnt 
		//eg. daVinci tank after death
		else if(weapon.ActiveWeapon.IsUsed)
		{
			Debug.LogError("Active weapon is still used");
			weapon.StopUseWeapon();
		}

	}

	private bool IsMovementRequested(EDirection pDirection)
	{
		if(pDirection != EDirection.None && pDirection == joystickDirection)
			return true;

		switch(pDirection)
		{
			case EDirection.Up:
				return Input.GetKey(keys.moveUp);
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
