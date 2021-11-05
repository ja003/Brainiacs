﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenu : GameMenuController
{
	[SerializeField] Text header;

	[SerializeField] Button btnClose;
	[SerializeField] Button btnBackground;

	[Header("Volume")]
	[SerializeField] Slider sliderVolumeMusic;
	[SerializeField] Slider sliderVolumeSounds;


	[SerializeField] Button btnResetSettings;
	[SerializeField] Button btnRespawn;

	[Header("End game")]
	[SerializeField] Button btnEndGame;
	[SerializeField] TextMeshProUGUI textEndGame;
	[SerializeField] GameObject holderEndGameConfirm;
	[SerializeField] Button btnConfirmYes;
	[SerializeField] Button btnConfirmNo;
	[SerializeField] Toggle toggleAllowAllDir;
	[SerializeField] Toggle toggleShowHealthbars;


	protected override void OnMainControllerAwaken() { }

	public override void SetActive(bool pValue)
	{
		base.SetActive(pValue);

		game.GameTime.SetPause(pValue && !isMultiplayer);
		header.text = $"Game {(game.GameTime.IsPaused ? "paused" : "running")}";
		holderEndGameConfirm.SetActive(false);
	}

	internal void Init()
	{
		SetVolumeSounds(brainiacs.PlayerPrefs.VolumeSounds);
		SetVolumeMusic(brainiacs.PlayerPrefs.VolumeMusic);

		btnClose.onClick.AddListener(() => SetActive(false));
		btnBackground.onClick.AddListener(() => SetActive(false));

		toggleAllowAllDir.onValueChanged.AddListener(OnAllowAllDirChanged);
		toggleAllowAllDir.isOn = brainiacs.PlayerPrefs.AllowMoveAllDir;
		toggleShowHealthbars.onValueChanged.AddListener(OnShowHealthbarsChanged);
		toggleShowHealthbars.isOn = brainiacs.PlayerPrefs.ShowHealthbars;


		//volume
		sliderVolumeMusic.onValueChanged.AddListener(SetVolumeMusic);
		sliderVolumeSounds.onValueChanged.AddListener(SetVolumeSounds);

		btnResetSettings.onClick.AddListener(OnResetClick);
		btnRespawn.onClick.AddListener(OnRespawnClick);

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

	private void OnShowHealthbarsChanged(bool pValue)
	{
		brainiacs.PlayerPrefs.ShowHealthbars = pValue;
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


	private void OnResetClick()
	{
		SetVolumeMusic(1);
		SetVolumeSounds(1);
		toggleAllowAllDir.isOn = true;
		toggleShowHealthbars.isOn = true;

		game.MobileInput.OnResetSettings();
	}


	private void OnRespawnClick()
	{
		string message = "Are you sure you want to respawn? You will lose 1 life.";
		string note = "use this when you get stuck or otherwise f*cked";
		UnityAction forceKillAction = () =>
		{
			//pause menu can be meanwhile closed via ESC
			bool isPauseMenuActive = IsActive();
			if(isPauseMenuActive)
				SetActive(false);

			DoInTime(() =>
			{
				Player player = game.PlayerManager.GetLocalPlayer();
				player.Health.ForceKill(false);
			}, isPauseMenuActive ? 0.5f : 0.1f, true);
			
		};
		game.Prompt.Show(message, note, true, forceKillAction);
	}
}
