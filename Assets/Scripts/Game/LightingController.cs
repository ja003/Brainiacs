using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class LightingController : GameBehaviour
{
	private const float DAY_MODE_TRANSITION_DURATION = 2f;

	private const float GLOBAL_LIGHT_INTENSITY_NIGHT = 0.2f;
	private const float GLOBAL_LIGHT_INTENSITY_DAY = 0.9f;

	private const float PLAYER_LIGHT_INTENSITY_NIGHT = 1f;
	private const float PLAYER_LIGHT_INTENSITY_DAY = 0.5f;
	private const float PLAYER_LIGHT_RADIUS_NIGHT = 2.5f;
	private const float PLAYER_LIGHT_RADIUS_DAY = 1.5f;

	[SerializeField] public UnityEngine.Rendering.Universal.Light2D GlobalLight;

	public bool IsNight;

	//time when night should end.
	//the value is updated with each SetNight call.
	//night cant end sooner that this time.
	//Value is updated only on master, clients dont handle lighting.
	float endNightTime;

	protected override void Awake()
	{
		base.Awake();
		UpdateLights(1);
	}

	public void SetNight(float pDuration)
	{
		endNightTime = Time.time + pDuration;
		if(!IsNight)
			SetMode(true);
		DoInTime(() => SetMode(false), pDuration);
	}

	public void SetMode(bool pNight)
	{
		//Debug.Log("SetMode " + pNight);

		if(!pNight && Time.time < endNightTime - 0.1f)
		{
			Debug.Log($"Dont turn back to day. {Time.time} < {endNightTime}");
			return;
		}

		IsNight = pNight;
		LeanTween.cancel(gameObject);
		DoInTime(null, DAY_MODE_TRANSITION_DURATION, false, UpdateLights);

		game.Photon.Send(EPhotonMsg.Game_Lighting_SetMode, pNight);
	}

	/// <summary>
	/// Lerp global + player light intesity + radius between day/night values.
	/// pProgress = <0,1>
	/// </summary>
	private void UpdateLights(float pProgress)
	{
		float t = IsNight ? pProgress : 1 - pProgress;

		float globalIntensity = Mathf.Lerp(GLOBAL_LIGHT_INTENSITY_DAY, GLOBAL_LIGHT_INTENSITY_NIGHT, t);
		GlobalLight.intensity = globalIntensity;

		float playerIntensity = Mathf.Lerp(PLAYER_LIGHT_INTENSITY_DAY, PLAYER_LIGHT_INTENSITY_NIGHT, t);
		foreach(Player player in game.PlayerManager.Players)
		{
			player.Visual.SetLightIntensity(playerIntensity);
			player.Visual.SetLightRadius(playerIntensity);
		}

		foreach(Tuple<Image, Color> affImage in affectedImages)
		{
			affImage.Item1.color = affImage.Item2 * globalIntensity;
			Utils.SetAlpha(affImage.Item1, affImage.Item2.a);
		}
	}

	List<Tuple<Image, Color>> affectedImages = new List<Tuple<Image, Color>>();


	internal void RegisterForLighting(Image pImage)
	{
		affectedImages.Add(new Tuple<Image, Color>(pImage, pImage.color));
	}
}