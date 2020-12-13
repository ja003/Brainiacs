using System;
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
		if(PlatformManager.GetPlatform() == EPlatform.PC)// && !DebugData.TestMobileInput)
		{
			if(pPlayerInfo.Keyset == EKeyset.None)
			{
				Debug.LogError($"{player} has no keyset");
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

			mobileInput.moveJoystick.OnUpdateDirection += HandleMoveJoystick;
		}
		isInited = true;
	}

	EDirection joystickDirection;
	private void HandleMoveJoystick(Vector2 pInput)
	{
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

	private void ProcessMovementInput()
	{
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

		//HACK
		if(Input.GetKeyDown(KeyCode.Backslash))
		{
			player.Stats.StatsEffect.ApplyEffect(EPlayerEffect.DoubleSpeed, 1);
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
