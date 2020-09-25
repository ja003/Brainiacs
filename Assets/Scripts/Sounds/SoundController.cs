using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundController
{
	static AudioManager audioManager => Brainiacs.Instance.AudioManager;
	static PlayerPrefsController playerPrefs => Brainiacs.Instance.PlayerPrefs;

	public static void PlaySound(ESound pKey, AudioSource pSource, bool pIsLoop = false)
	{
		if(pSource == null)
			pSource = audioManager.AudioSourceNormal;

		AudioClip clip = audioManager.GetSoundClip(pKey);
		TryPlay(pSource, clip, false, pIsLoop);
	}

	public static void PlayWeaponUseSound(EWeaponId pWeapon, AudioSource pSource, bool pIsLoop = false)
	{
		AudioClip clip = audioManager.GetWeaponUseClip(pWeapon);
		TryPlay(pSource, clip, false, pIsLoop);
	}

	public static void PlayMusic(ESound pSound)
	{
		AudioSource source = audioManager.AudioSourceMusic;
		AudioClip clip = audioManager.GetSoundClip(pSound);
		TryPlay(source, clip, true, true);
	}

	/// <summary>
	/// All audio source should be started here.
	/// Sets the clip, volume, etc
	/// </summary>
	private static void TryPlay(AudioSource source, AudioClip clip, bool pIsMusic, bool pIsLoop)
	{
		if(clip == null)
		{
			Debug.LogError($"Clip is null");
			return;
		}
		if(Brainiacs.Instance.AudioManager.debug_LogSounds)
			Debug.Log($"Play: {clip.name}");

		float musicVolume = DebugData.MuteMusic ? 0 : playerPrefs.VolumeMusic;

		source.volume = pIsMusic ? musicVolume : playerPrefs.VolumeSounds;
		if(pIsMusic) source.Stop();

		//source.clip = clip;
		//source.loop = pIsLoop;
		//source.Play(); //no! this stops previously played clip
		if(pIsLoop)
		{
			source.clip = clip;
			source.loop = pIsLoop;
			source.Play();
		}
		else
		{
			source.PlayOneShot(clip);
		}

	}
}

public enum ESound
{
	None = 0,

	Music_Menu = 10,
	Music_Game = 11,

	Loading_Beep_1 = 20,
	Loading_Beep_2 = 21,
	Loading_Beep_3 = 22,
	Loading_Beep_4 = 23,

	Ui_Button_Click = 50,
	Ui_Button_Hover = 51,
	Ui_Button_Wrong = 52,

	debug_Shoot = 666,

	Curie_Truck_Ride = 101,
	Curie_Truck_Explode = 102,
	Einstein_Explosion = 103,
	Davinci_Tank_Hit = 104,
	Nobel_Mine_Explode = 105,


	Item_Spawn = 201,
	Item_Explode = 202,
	//
	Item_Powerup_Ammo = 203,
	Item_Powerup_Heal = 204,
	Item_Powerup_ReceiveDamage = 205,
	Item_Powerup_Shield = 206,
	Item_Powerup_Slow = 207,
	Item_Powerup_Speed = 208,
	Item_Powerup_DoubleDamage = 209,
	Item_Powerup_HalfDamage = 210,
	//
	Item_Weapon_Pickup = 211,


	Player_Hit = 300,
	Player_Shield_Hit = 301,
	Player_Eliminate = 302,

	Map_Obstackle_Hit = 400,
}
