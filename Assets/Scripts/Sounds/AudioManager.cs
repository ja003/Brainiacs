using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[SerializeField] public AudioSource AudioSourceNormal;
	[SerializeField] public AudioSource AudioSourceMusic;

	[SerializeField] List<SoundAudioClip> _soundClips;
	[SerializeField] List<SoundAudioClip> _musicSoundClips;
	[SerializeField] List<SoundAudioClip> _itemSoundClips;
	[SerializeField] List<SoundAudioClip> _specialWeaponSounds;
	Dictionary<ESound, AudioClip> soundClips = new Dictionary<ESound, AudioClip>();

	[SerializeField] List<WeaponAudioClip> _weaponUseClips;
	Dictionary<EWeaponId, AudioClip> weaponUseClips = new Dictionary<EWeaponId, AudioClip>();

	
	//Dictionary<ESound, AudioClip> otherSoundClips = new Dictionary<ESound, AudioClip>();

	private void Awake()
	{
		Init();
	}

	bool isInited;
	private void Init()
	{
		if(isInited) return;

		List<SoundAudioClip> allSoundClips = new List<SoundAudioClip>();
		allSoundClips.AddRange(_soundClips);
		allSoundClips.AddRange(_musicSoundClips);
		allSoundClips.AddRange(_itemSoundClips);
		allSoundClips.AddRange(_specialWeaponSounds);

		foreach(var sc in allSoundClips)
		{
			if(sc.clip == null || sc.key == ESound.None)
				continue;
			soundClips.Add(sc.key, sc.clip);
		}


		foreach(var weapon in _weaponUseClips)
		{
			if(weapon.clip == null || weapon.key == EWeaponId.None)
				continue;

			weaponUseClips.Add(weapon.key, weapon.clip);
		}

		//foreach(var sc in _otherSoundClips)
		//{
		//	if(sc.clip == null || sc.key == ESound.None)
		//		continue;
		//	otherSoundClips.Add(sc.key, sc.clip);
		//}

		isInited = true;
	}

	public AudioClip GetSoundClip(ESound pKey)
	{
		if(!isInited)
			Init();

		AudioClip clip;
		soundClips.TryGetValue(pKey, out clip);
		if(clip == null)
			Debug.LogError($"Sound {pKey} not found");

		return clip;
	}

	public AudioClip GetWeaponUseClip(EWeaponId pKey)
	{
		if(!isInited)
			Init();

		AudioClip clip;
		weaponUseClips.TryGetValue(pKey, out clip);
		if(clip == null)
			Debug.LogError($"Sound {pKey} not found");

		return clip;
	}

}

[Serializable]
public class SoundAudioClip
{
	public ESound key;
	public AudioClip clip;
}

[Serializable]
public class WeaponAudioClip
{
	public EWeaponId key;
	public AudioClip clip;
}
