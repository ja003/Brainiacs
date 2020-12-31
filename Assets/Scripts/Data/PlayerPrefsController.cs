using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsController : GameBehaviour
{
	private const string KEY_VOLUME_MUSIC = "VOLUME_MUSIC";
	public float VolumeMusic
	{
		get { return PlayerPrefs.GetFloat(KEY_VOLUME_MUSIC, .2f); }
		set { PlayerPrefs.SetFloat(KEY_VOLUME_MUSIC, value); }
	}

	private const string KEY_VOLUME_SOUNDS = "VOLUME_SOUNDS";
	public float VolumeSounds
	{
		get { return PlayerPrefs.GetFloat(KEY_VOLUME_SOUNDS, .2f); }
		set { PlayerPrefs.SetFloat(KEY_VOLUME_SOUNDS, value); }
	}

	private const string KEY_MOVE_INPUT_SCALE = "MOVE_INPUT_SCALE";
	public float MoveInputScale
	{
		get { return PlayerPrefs.GetFloat(KEY_MOVE_INPUT_SCALE, 1); }
		set { PlayerPrefs.SetFloat(KEY_MOVE_INPUT_SCALE, value); }
	}

	private const string KEY_MOVE_INPUT_POS = "MOVE_INPUT_POS";
	public Vector2 MoveInputPos
	{
		get { return PlayerPrefsX.GetVector2(KEY_MOVE_INPUT_POS); }
		set { PlayerPrefsX.SetVector2(KEY_MOVE_INPUT_POS, value); }
	}

	private const string KEY_ALLOW_MOVE_ALL_DIR = "KEY_ALLOW_MOVE_ALL_DIR";
	public bool AllowMoveAllDir
	{
		get { return PlayerPrefsX.GetBool(KEY_ALLOW_MOVE_ALL_DIR); }
		set { PlayerPrefsX.SetBool(KEY_ALLOW_MOVE_ALL_DIR, value); }
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

	public static Vector2 GetVector2(string name)
	{
		if(PlayerPrefs.HasKey(name + "_X") && PlayerPrefs.HasKey(name + "_Y"))
		{
			float x = PlayerPrefs.GetFloat(name + "_X");
			float y = PlayerPrefs.GetFloat(name + "_Y");
			return new Vector2(x, y);
		}
		return Vector2.zero;
	}

	public static void SetVector2(string name, Vector2 pValue)
	{
		PlayerPrefs.SetFloat(name + "_X", pValue.x);
		PlayerPrefs.SetFloat(name + "_Y", pValue.y);
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