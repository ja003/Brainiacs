using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : GameController
{
	[SerializeField] Text header;

	[SerializeField] Button btnClose;

	[Header("Volume")]
	[SerializeField] Slider sliderVolumeMusic;
	[SerializeField] Slider sliderVolumeSounds;

	[Header("Mobile input")]
	[SerializeField] GameObject inputType;
	[SerializeField] Button btnInputJoystick;
	[SerializeField] Button btnInputButtons;
	[SerializeField] GameObject inputScale;
	[SerializeField] Slider sliderMoveInputScale;


	protected override void OnMainControllerAwaken() { }

	internal void Init()
	{
		SetMobileInputJoystick(brainiacs.PlayerPrefs.MobileInputJoystick);
		SetVolumeSounds(brainiacs.PlayerPrefs.VolumeSounds);
		SetVolumeMusic(brainiacs.PlayerPrefs.VolumeMusic);
		SetMoveInputScale(brainiacs.PlayerPrefs.MoveInputScale);

		btnClose.onClick.AddListener(() => SetActive(false));
		
		//volume
		sliderVolumeMusic.onValueChanged.AddListener(SetVolumeMusic);
		sliderVolumeSounds.onValueChanged.AddListener(SetVolumeSounds);

		//mobile input
		if(PlatformManager.IsMobile())
		{
			sliderMoveInputScale.onValueChanged.AddListener(SetMoveInputScale);
			btnInputButtons.onClick.AddListener(() => SetMobileInputJoystick(false));
			btnInputJoystick.onClick.AddListener(() => SetMobileInputJoystick(true));
		}
		else
		{
			inputType.SetActive(false);
			inputScale.SetActive(false);
		}

	}

	private void SetMoveInputScale(float pValue)
	{
		brainiacs.PlayerPrefs.MoveInputScale = pValue;
		sliderMoveInputScale.value = pValue;
		game.MobileInput.SetMoveInputScale(pValue);
	}

	private void SetVolumeMusic(float pValue)
	{
		//Debug.Log("SetVolumeMusic " + pValue);
		brainiacs.PlayerPrefs.VolumeMusic = pValue;

		//audio source volume has to be set (music starts only once)		
		brainiacs.AudioManager.AudioSourceMusic.volume = pValue;

		sliderVolumeMusic.value = pValue;
	}

	private void SetVolumeSounds(float pValue)
	{
		//Debug.Log("SetVolumeSounds " + pValue);
		brainiacs.PlayerPrefs.VolumeSounds = pValue;
		//no need to set volume to audio source, it is set when it is played
		sliderVolumeSounds.value = pValue;
	}

	private void SetMobileInputJoystick(bool pValue)
	{
		//Debug.Log("SetMobileInputJoystick " + pValue);

		brainiacs.PlayerPrefs.MobileInputJoystick = pValue;
		btnInputButtons.image.color = pValue ? Color.gray : Color.white;
		btnInputJoystick.image.color = !pValue ? Color.gray : Color.white;

		if(PlatformManager.IsMobile())
			game.MobileInput.SetActive(true, pValue);
	}

	protected override void OnSetActive(bool pValue)
	{
		bool isMP = brainiacs.GameInitInfo.IsMultiplayer();
		header.text = $"Game {(isMP ? "not " : "")}paused";
		Time.timeScale = pValue && !isMP ? 0 : 1;

		base.OnSetActive(pValue);
	}
}
