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
}
