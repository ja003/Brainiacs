using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

	[Header("End game")]
	[SerializeField] Button btnEndGame;
	[SerializeField] TextMeshProUGUI textEndGame;
	[SerializeField] GameObject holderEndGameConfirm;
	[SerializeField] Button btnConfirmYes;
	[SerializeField] Button btnConfirmNo;


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

		//end game
		holderEndGameConfirm.SetActive(false);
		btnConfirmNo.onClick.AddListener(OnConfirmNoClick);
		btnEndGame.onClick.AddListener(OnEndGameClick);
		//only master can end the game, clients can only leave the room
		bool canPlayerEndGame = brainiacs.PhotonManager.IsMaster();
		if(canPlayerEndGame)
		{
			textEndGame.text = "End game";
			btnConfirmYes.onClick.AddListener(OnEndGameConfirmYesClick);
		}
		else
		{
			textEndGame.text = "Leave game";
			btnConfirmYes.onClick.AddListener(OnLeaveGameConfirmYesClick);
		}

	}

	/// <summary>
	/// Leave room and loads the result scene.
	/// Called only from client.
	/// </summary>
	private void OnLeaveGameConfirmYesClick()
	{
		if(brainiacs.PhotonManager.IsMaster())
			Debug.LogError("Leave game shouldn't be called from master");

		Debug.Log("LEAVE GAME");

		if(!isMultiplayer)
			Debug.LogError("Leave game should be called only in multiplayer");
		else
			PhotonNetwork.LeaveRoom();

		game.uiCurtain.SetFade(true, () => brainiacs.Scenes.LoadScene(EScene.MainMenu));
	}

	/// <summary>
	/// Send end game info to other players, close curtain and load the result scene.
	/// Called only from master.
	/// </summary>
	private void OnEndGameConfirmYesClick()
	{
		if(!brainiacs.PhotonManager.IsMaster())
			Debug.LogError("End game should be called only from master");

		Debug.Log("END GAME");
		game.GameEnd.EndGame();
	}

	/// <summary>
	/// Hides confirm options.
	/// For bot end/leave game.
	/// </summary>
	private void OnConfirmNoClick()
	{
		holderEndGameConfirm.SetActive(false);
	}

	/// <summary>
	/// Shows confirm options.
	/// For bot end/leave game.
	/// </summary>
	private void OnEndGameClick()
	{
		holderEndGameConfirm.SetActive(true);
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
		game.GameTime.IsPaused = pValue && !isMultiplayer;
		header.text = $"Game {(game.GameTime.IsPaused ? "" : "not ")}paused";
		Time.timeScale = game.GameTime.IsPaused ? 0 : 1;

		base.OnSetActive(pValue);
	}
}
