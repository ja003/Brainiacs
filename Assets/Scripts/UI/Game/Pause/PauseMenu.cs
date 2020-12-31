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


	[SerializeField] Button btnResetSettings;

	[Header("End game")]
	[SerializeField] Button btnEndGame;
	[SerializeField] TextMeshProUGUI textEndGame;
	[SerializeField] GameObject holderEndGameConfirm;
	[SerializeField] Button btnConfirmYes;
	[SerializeField] Button btnConfirmNo;
	[SerializeField] Toggle toggleAllowAllDir;


	protected override void OnMainControllerAwaken() { }

	protected override void OnSetActive(bool pValue)
	{
		game.GameTime.SetPause(pValue && !isMultiplayer);
		header.text = $"Game {(game.GameTime.IsPaused ? "" : "not ")}paused";
		holderEndGameConfirm.SetActive(false);

		base.OnSetActive(pValue);
	}

	internal void Init()
	{
		SetVolumeSounds(brainiacs.PlayerPrefs.VolumeSounds);
		SetVolumeMusic(brainiacs.PlayerPrefs.VolumeMusic);

		btnClose.onClick.AddListener(() => SetActive(false));

		toggleAllowAllDir.onValueChanged.AddListener(OnAllowAllDirChanged);
		toggleAllowAllDir.isOn = brainiacs.PlayerPrefs.AllowMoveAllDir;

		//volume
		sliderVolumeMusic.onValueChanged.AddListener(SetVolumeMusic);
		sliderVolumeSounds.onValueChanged.AddListener(SetVolumeSounds);

		btnResetSettings.onClick.AddListener(ResetSettings);

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

	private void OnAllowAllDirChanged(bool pValue)
	{
		brainiacs.PlayerPrefs.AllowMoveAllDir = pValue;
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

	private void SetVolumeMusic(float pValue)
	{
		//Debug.Log("SetVolumeMusic " + pValue);
		brainiacs.PlayerPrefs.VolumeMusic = pValue;

		//audio source volume has to be updated (music starts only once)		
		brainiacs.AudioManager.UpdateAudioVolume(brainiacs.AudioManager.AudioSourceMusic, pValue);

		sliderVolumeMusic.value = pValue;
	}

	private void SetVolumeSounds(float pValue)
	{
		//Debug.Log("SetVolumeSounds " + pValue);
		brainiacs.PlayerPrefs.VolumeSounds = pValue;
		//no need to set volume to audio source, it is set when it is played
		sliderVolumeSounds.value = pValue;
	}


	private void ResetSettings()
	{
		SetVolumeMusic(1);
		SetVolumeSounds(1);
		game.MobileInput.OnResetSettings();
	}
}
