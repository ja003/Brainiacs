using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInput : GameBehaviour
{
	[SerializeField] public ExtendedButton btnShoot;
	[SerializeField] public ExtendedButton btnSwap;
	[SerializeField] public MoveJoystick moveJoystick;

	protected override void Awake()
	{
		base.Awake();
		//DONT! screen is not inited yet? and setting element position works weird
		//Init();
	}

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		SetActive(PlatformManager.IsMobile());
		SetMoveInputScale(brainiacs.PlayerPrefs.MoveInputScale);

		RefreshMoveInputTransform();

		game.GameEnd.OnGameEnd += SaveInputTransform;
		game.GameTime.OnSetPause += OnSetPause;
	}

	private void OnApplicationPause(bool pause)
	{
		SaveInputTransform();
	}

	private void OnApplicationQuit()
	{
		SaveInputTransform();
	}

	public void SetActive(bool pValue)
	{
		moveJoystick.gameObject.SetActive(pValue);

		btnShoot.gameObject.SetActive(pValue);
		btnSwap.gameObject.SetActive(pValue);
	}

	public void SetMoveInputScale(float pValue)
	{
		moveJoystick.transform.localScale = Vector3.one * pValue;
	}

	//position of joystick and move buttons is stored together

	private void RefreshMoveInputTransform()
	{
		if(brainiacs.PlayerPrefs.MoveInputPos == Vector2.zero)
		{
			Debug.Log("MoveInputPos not set");
			brainiacs.PlayerPrefs.MoveInputPos = GetMoveInputPosition();
		}

		//Debug.Log("RefreshMoveInputTransform " + brainiacs.PlayerPrefs.MoveInputPos + ", " + brainiacs.PlayerPrefs.MoveInputScale);

		moveJoystick.GetComponent<RectTransform>().position = brainiacs.PlayerPrefs.MoveInputPos;
		moveJoystick.GetComponent<RectTransform>().localScale = brainiacs.PlayerPrefs.MoveInputScale * Vector3.one;
	}

	private Vector2 GetMoveInputPosition()
	{
		return moveJoystick.GetComponent<RectTransform>().position;
	}

	private Vector3 GetMoveInputScale()
	{
		return moveJoystick.GetComponent<RectTransform>().localScale;
	}

	private void SaveInputTransform()
	{
		brainiacs.PlayerPrefs.MoveInputPos = GetMoveInputPosition();
		brainiacs.PlayerPrefs.MoveInputScale = GetMoveInputScale().x;
		//Debug.Log("SaveInputTransform " + brainiacs.PlayerPrefs.MoveInputPos + ", " + brainiacs.PlayerPrefs.MoveInputScale);
	}

	private void OnSetPause(bool pIsPaused)
	{
		if(pIsPaused)
			SaveInputTransform();
	}

	internal void OnResetSettings()
	{
		Debug.Log("Reset settings");
		//reset to default position and scale
		brainiacs.PlayerPrefs.MoveInputPos = new Vector2(225, 200);
		brainiacs.PlayerPrefs.MoveInputScale = 1;
		RefreshMoveInputTransform();
	}
}
