using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCamera : BrainiacsBehaviour
{
	const float coef = 0.01f; //not sure why this is needed
	//game has a fixed resolution
	const int widthToBeSeen = 1920;
	const int heightToBeSeen = 1080;
	const float ideal_ratio = (float)widthToBeSeen / heightToBeSeen;


	Camera mainCamera;
	Vector2 resolution;

	private void Start()
	{
		mainCamera = GetComponent<Camera>();
		resolution = new Vector2(Screen.width, Screen.height);
		UpdateOrtoSize();
	}

	private void Update()
	{
		bool resolutionChanged =
			resolution.x != Screen.width || resolution.y != Screen.height;
		if(resolutionChanged)
		{
			UpdateOrtoSize();
		}
	}

	private void UpdateOrtoSize()
	{
		resolution.x = Screen.width;
		resolution.y = Screen.height;

		float currentRatio = (float)Screen.width / Screen.height;
		
		float newSize;
		//screen has to be fit by height or width
		if(currentRatio > ideal_ratio)
		{
			//https://answers.unity.com/questions/1569387/resizing-orthographic-camera-to-fit-2d-sprite-on-s-1.html
			newSize = coef * heightToBeSeen * 0.5f;
		}
		else
		{
			//https://answers.unity.com/questions/760671/resizing-orthographic-camera-to-fit-2d-sprite-on-s.html
			newSize = coef * widthToBeSeen * Screen.height / Screen.width* 0.5f;
		}
		mainCamera.orthographicSize = newSize;

		Debug.Log(
			$"orto:{mainCamera.orthographicSize}, " +
				$"{ Screen.width} x { Screen.height}, " +
				$"ratio: {currentRatio} | {ideal_ratio}");
	}
}
