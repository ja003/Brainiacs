using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightingController : GameBehaviour
{
	private const float DAY_MODE_TRANSITION_DURATION = 2f;

	private const float GLOBAL_LIGHT_INTENSITY_NIGHT = 0.2f;
	private const float GLOBAL_LIGHT_INTENSITY_DAY = 0.9f;

	private const float PLAYER_LIGHT_INTENSITY_NIGHT = 1f;
	private const float PLAYER_LIGHT_INTENSITY_DAY = 0.5f;
	private const float PLAYER_LIGHT_RADIUS_NIGHT = 2.5f;
	private const float PLAYER_LIGHT_RADIUS_DAY = 1.5f;

	[SerializeField] Light2D globalLight;

	public bool IsNight;

	//time when night should end.
	//the value is updated with each SetNight call.
	//night cant end sooner that this time.
	float endNightTime;

	public void SetNight(float pDuration)
	{
		Debug.LogError("TODO: MP msg");

		endNightTime = Time.time + pDuration;
		if(!IsNight)
			SetMode(true);
		DoInTime(() => SetMode(false), pDuration);
	}

	public void SetMode(bool pNight)
	{
		if(!pNight && Time.time < endNightTime - 0.1f)
		{
			Debug.Log($"Dont turn back to day. {Time.time} < {endNightTime}");
			return;
		}

		IsNight = pNight;
		DoInTime(null, DAY_MODE_TRANSITION_DURATION, false, UpdateLights);
	}

	/// <summary>
	/// Lerp global + player light intesity + radius between day/night values.
	/// pProgress = <0,1>
	/// </summary>
	private void UpdateLights(float pProgress)
	{
		float t = IsNight ? pProgress : 1 - pProgress;
		float intensity = Mathf.Lerp(GLOBAL_LIGHT_INTENSITY_DAY, GLOBAL_LIGHT_INTENSITY_NIGHT, t);
		globalLight.intensity = intensity;

		intensity = Mathf.Lerp(PLAYER_LIGHT_INTENSITY_DAY, PLAYER_LIGHT_INTENSITY_NIGHT, t);
		foreach(Player player in game.PlayerManager.Players)
		{
			player.Visual.SetLightIntensity(intensity);
			player.Visual.SetLightRadius(intensity);
		}
	}

}