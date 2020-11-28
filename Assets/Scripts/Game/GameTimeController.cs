using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control game time passed and time left.
/// If game mode = time => triggers end of game.
/// TODO: might have to implement gameTimeScale
/// </summary>
public class GameTimeController : GameController
{
	[SerializeField] UIGameTime uiGameTime = null;

	public bool IsPaused { get; private set; }
	public Action<bool> OnSetPause;

	public void SetPause(bool pIsPaused)
	{
		Time.timeScale = pIsPaused ? 0 : 1;
		IsPaused = pIsPaused;
		OnSetPause?.Invoke(IsPaused);
		SoundController.OnSetPause(IsPaused);
	}

	protected override void OnMainControllerAwaken()
	{
	}

	protected override void OnMainControllerActivated()
	{
		if(isMultiplayer && !PhotonNetwork.IsMasterClient)
		{
			//master manages time
			return;
		}

		game.OnGameStarted += StartTime;
	}

	int timeLeft;
	public int TimePassed { get; private set; }
	EGameMode mode => brainiacs.GameInitInfo.Mode;

	private void StartTime()
	{
		Debug.Log("StartTime");
		if(mode == EGameMode.Time)
		{
			timeLeft = brainiacs.GameInitInfo.GameModeValue * 60;
		}
		Tick();
	}

	private void Tick()
	{
		if(game.GameEnd.GameEnded)
			return;

		TimePassed++;
		if(mode == EGameMode.Time)
		{
			timeLeft--;
			if(timeLeft <= 0)
			{
				game.GameEnd.EndGame();
			}
		}
		//Debug.Log($"TICK {timePassed}/{timeLeft}");

		//float nextTick = Time.timeScale > 0 ? 1 / Time.timeScale : 1;
		float nextTick = 1; //for now we work with Time.timeScale not with gameTimScale
		DoInTime(Tick, nextTick);

		uiGameTime.UpdateTime(TimePassed, timeLeft);
	}
}
