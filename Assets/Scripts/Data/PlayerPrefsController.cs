using System;
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

	public PlayerKeys GetPlayerKeys(EKeyset pKeyset)
	{
		KeyCode up = PlayerPrefsX.GetKeysetKey(pKeyset, EActionKey.Up);
		KeyCode right = PlayerPrefsX.GetKeysetKey(pKeyset, EActionKey.Right);
		KeyCode down = PlayerPrefsX.GetKeysetKey(pKeyset, EActionKey.Down);
		KeyCode left = PlayerPrefsX.GetKeysetKey(pKeyset, EActionKey.Left);

		KeyCode swap = PlayerPrefsX.GetKeysetKey(pKeyset, EActionKey.Swap);
		KeyCode use = PlayerPrefsX.GetKeysetKey(pKeyset, EActionKey.Use);

		PlayerKeys keys = new PlayerKeys(up, right, down, left, swap, use);
		if(!keys.IsValid())
		{
			Debug.LogError($"{pKeyset} not set. is this first run?");
			keys = new PlayerKeys(pKeyset);
		}

		return keys;
	}

	public void SetPlayerKeys(EKeyset pKeyset, PlayerKeys pKeys)
	{
		SetPlayerActionKey(pKeyset, EActionKey.Up, pKeys.moveUp);
		SetPlayerActionKey(pKeyset, EActionKey.Right, pKeys.moveRight);
		SetPlayerActionKey(pKeyset, EActionKey.Down, pKeys.moveDown);
		SetPlayerActionKey(pKeyset, EActionKey.Left, pKeys.moveLeft);
		SetPlayerActionKey(pKeyset, EActionKey.Swap, pKeys.swapWeapon);
		SetPlayerActionKey(pKeyset, EActionKey.Use, pKeys.useWeapon);
	}

	private void SetPlayerActionKey(EKeyset pKeyset, EActionKey pKey, KeyCode pKeyCode)
	{
		PlayerPrefs.SetInt(PlayerPrefsX.GenerateKeysetActionName(pKeyset, pKey), (int)pKeyCode);
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

	internal static KeyCode GetKeysetKey(EKeyset pKeyset, EActionKey pAction)
	{
		string name = GenerateKeysetActionName(pKeyset, pAction);
		if(!PlayerPrefs.HasKey(name))
			return KeyCode.None;

		return (KeyCode)PlayerPrefs.GetInt(name);
	}

	public static string GenerateKeysetActionName(EKeyset pKeyset, EActionKey pAction)
	{
		return pKeyset.ToString() + pAction.ToString();
	}
}