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
	/// Input from keyboard and physical joystick.
	/// small BUG: if holding 2 opposite keys (<-, ->) one direction
	/// is has bigger impact
	/// </summary>
	private void CalculateMoveKeys()
	{
		const float move_change_speed = .25f;
		bool movementRequested = false;

		const float input_thr = 0.1f;
		float vertical = GetJoystickInput("Vertical");
		float horizontal = GetJoystickInput("Horizontal");

		//Debug.Log($"{player.InitInfo.Keyset} = {horizontal} , {vertical}");

		if(Input.GetKey(keys.moveUp) || vertical > input_thr)
		{
			//has to be registered in every case
			movementRequested = true;
			//set to zero if going down for fast direction change
			if(keyDirection.y < 0)
				keyDirection.y = 0;
			keyDirection += Vector2.up * move_change_speed;
		}
		if(Input.GetKey(keys.moveRight) || horizontal > input_thr)
		{
			if(keyDirection.x < 0)
				keyDirection.x = 0;
			movementRequested = true;
			keyDirection += Vector2.right * move_change_speed;
		}
		if(Input.GetKey(keys.moveDown) || vertical < -input_thr)
		{
			if(keyDirection.y > 0)
				keyDirection.y = 0;
			movementRequested = true;
			keyDirection += Vector2.down * move_change_speed;
		}
		if(Input.GetKey(keys.moveLeft) || horizontal < -input_thr)
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

		if(Input.GetKeyDown(keys.swapWeapon) || Input.GetKeyDown(GetJoystickKeyCode(EActionKey.Swap)))
			weapon.SwapWeapon();

		if(Input.GetKeyUp(keys.useWeapon) || Input.GetKeyUp(GetJoystickKeyCode(EActionKey.Use)))
			weapon.StopUseWeapon();

		if(Input.GetKey(keys.useWeapon) || Input.GetKey(GetJoystickKeyCode(EActionKey.Use)))
			weapon.UseWeapon();
		//auto detect if weapon is used when shouldnt 
		//eg. daVinci tank after death
		else if(weapon.ActiveWeapon.IsUsed)
		{
			Debug.LogError("Active weapon is still used");
			weapon.StopUseWeapon();
		}

	}

	/// <summary>
	/// Returns KeyCode for use or swap action of physical joystick controller (not the virtal joystick)
	/// </summary>
	private KeyCode GetJoystickKeyCode(EActionKey pAction)
	{
		switch(pAction)
		{
			case EActionKey.Swap:
				switch(player.InitInfo.Keyset)
				{
					case EKeyset.KeysetA:
						return KeyCode.Joystick1Button4;
					case EKeyset.KeysetB:
						return KeyCode.Joystick2Button4;
					case EKeyset.KeysetC:
						return KeyCode.Joystick3Button4;
					case EKeyset.KeysetD:
						return KeyCode.Joystick4Button4;
				}
				Debug.LogError("No valid keyset");
				return KeyCode.Joystick1Button4;

			case EActionKey.Use:
				switch(player.InitInfo.Keyset)
				{
					case EKeyset.KeysetA:
						return KeyCode.Joystick1Button5;
					case EKeyset.KeysetB:
						return KeyCode.Joystick2Button5;
					case EKeyset.KeysetC:
						return KeyCode.Joystick3Button5;
					case EKeyset.KeysetD:
						return KeyCode.Joystick4Button5;
				}
				Debug.LogError("No valid keyset");
				return KeyCode.Joystick1Button5;
		}

		Debug.LogError("No button found");
		return KeyCode.None;
	}

	/// <summary>
	/// Input from physical joystick controller (not the virtal joystick)
	/// </summary>
	private float GetJoystickInput(string pName)
	{
		switch(player.InitInfo.Keyset)
		{
			case EKeyset.KeysetA:
				pName += "A";
				break;
			case EKeyset.KeysetB:
				pName += "B";
				break;
			case EKeyset.KeysetC:
				pName += "C";
				break;
			case EKeyset.KeysetD:
				pName += "D";
				break;
		}
		//Debug.Log($"{pName} = {Input.GetAxis(pName)}");

		return Input.GetAxis(pName);
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
