using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerKeys
{
	public KeyCode moveUp;
	public KeyCode moveRight;
	public KeyCode moveDown;
	public KeyCode moveLeft;

	public KeyCode useWeapon;
	public KeyCode swapWeapon;

	public PlayerKeys(KeyCode moveUp, KeyCode moveRight, KeyCode moveDown, KeyCode moveLeft, KeyCode useWeapon, KeyCode swapWeapon)
	{
		this.moveUp = moveUp;
		this.moveRight = moveRight;
		this.moveDown = moveDown;
		this.moveLeft = moveLeft;
		this.useWeapon = useWeapon;
		this.swapWeapon = swapWeapon;
	}

	internal bool IsValid()
	{
		return 
			moveUp != KeyCode.None &&
			moveRight != KeyCode.None &&
			moveDown != KeyCode.None &&
			moveLeft != KeyCode.None &&
			swapWeapon != KeyCode.None &&
			useWeapon != KeyCode.None
			;
	}

	public PlayerKeys(EKeyset pDefaultValues)
	{
		switch(pDefaultValues)
		{
			case EKeyset.KeysetA:
				moveUp = KeyCode.W;
				moveRight = KeyCode.D;
				moveDown = KeyCode.S;
				moveLeft = KeyCode.A;
				useWeapon = KeyCode.LeftControl;
				swapWeapon = KeyCode.LeftShift;
				break;
			case EKeyset.KeysetB:
				moveUp = KeyCode.UpArrow;
				moveRight = KeyCode.RightArrow;
				moveDown = KeyCode.DownArrow;
				moveLeft = KeyCode.LeftArrow;
				useWeapon = KeyCode.RightControl;
				swapWeapon = KeyCode.RightShift;
				break;
			case EKeyset.KeysetC:
				moveUp = KeyCode.I;
				moveRight = KeyCode.L;
				moveDown = KeyCode.K;
				moveLeft = KeyCode.J;
				useWeapon = KeyCode.O;
				swapWeapon = KeyCode.P;

				break;
			case EKeyset.KeysetD:
				moveUp = KeyCode.Keypad8;
				moveRight = KeyCode.Keypad6;
				moveDown = KeyCode.Keypad5;
				moveLeft = KeyCode.Keypad4;
				useWeapon = KeyCode.Keypad1;
				swapWeapon = KeyCode.Keypad0;

				break;
			default:
				moveUp = KeyCode.None;
				moveRight = KeyCode.None;
				moveDown = KeyCode.None;
				moveLeft = KeyCode.None;
				useWeapon = KeyCode.None;
				swapWeapon = KeyCode.None;
				break;
		}
	}
}
