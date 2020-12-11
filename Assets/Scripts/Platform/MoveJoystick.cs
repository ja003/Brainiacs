using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveJoystick : Joystick
{
	public Action<Vector2> OnUpdateDirection;

	//protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
	//{
	//	base.HandleInput(magnitude, normalised, radius, cam);
	//	Debug.Log(normalised);

	//	OnMoveAction?.Invoke(normalised);
	//}

	private void FixedUpdate()
	{
		OnUpdateDirection?.Invoke(Direction);

	}
}
