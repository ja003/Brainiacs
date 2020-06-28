using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsController : GameBehaviour
{
	private const string KEY_MOBILE_INPUT_JOYSTICK = "MOBILE_INPUT_JOYSTICK";
	public bool MobileInputJoystick
	{
		get { return PlayerPrefsX.GetBool(KEY_MOBILE_INPUT_JOYSTICK, false); }
		set { PlayerPrefsX.SetBool(KEY_MOBILE_INPUT_JOYSTICK, value); }
	}

	private const string KEY_VOLUME_MUSIC = "VOLUME_MUSIC";
	public float VolumeMusic
	{
		get { return PlayerPrefs.GetFloat(KEY_VOLUME_MUSIC, 1); }
		set { PlayerPrefs.SetFloat(KEY_VOLUME_MUSIC, value); }
	}

	private const string KEY_VOLUME_SOUNDS = "VOLUME_SOUNDS";
	public float VolumeSounds
	{
		get { return PlayerPrefs.GetFloat(KEY_VOLUME_SOUNDS, 1); }
		set { PlayerPrefs.SetFloat(KEY_VOLUME_SOUNDS, value); }
	}

	private const string KEY_MOVE_INPUT_SCALE = "MOVE_INPUT_SCALE";
	public float MoveInputScale
	{
		get { return PlayerPrefs.GetFloat(KEY_MOVE_INPUT_SCALE, 1); }
		set { PlayerPrefs.SetFloat(KEY_MOVE_INPUT_SCALE, value); }
	}

}


public class PlayerPrefsX
{
	public static void SetBool(string name, bool booleanValue)
	{
		PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
	}

	public static bool GetBool(string name)
	{
		return PlayerPrefs.GetInt(name) == 1 ? true : false;
	}

	public static bool GetBool(string name, bool defaultValue)
	{
		if(PlayerPrefs.HasKey(name))
		{
			return GetBool(name);
		}

		return defaultValue;
	}
}