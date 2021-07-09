using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : BrainiacsBehaviour
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

	PlayerPrefsController playerPrefs => Brainiacs.Instance.PlayerPrefs;

	public bool debug_LogSounds;

	//Dictionary<ESound, AudioClip> otherSoundClips = new Dictionary<ESound, AudioClip>();

	protected override void Awake()
	{
		base.Awake();
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

	// *** PLAY SOUND *** //

	List<AudioSource> usedSources = new List<AudioSource>();

	//globally reduce all sounds (all seem to be set unconfortably loud)
	private const float VOLUME_REDUCTION = 7f;

	public void OnSetPause(bool pIsPaused)
	{
		for(int i = usedSources.Count - 1; i >= 0; i--)
		{
			AudioSource s = usedSources[i];
			if(s == null)
			{
				usedSources.RemoveAt(i);
				continue;
			}


			if(pIsPaused)
			{
				//music never stops
				if(s == AudioSourceMusic)
					continue;

				//sources that are not playing are not used anymore
				if(!s.isPlaying)
				{
					usedSources.RemoveAt(i);
					continue;
				}

				s.Pause();
			}
			else
			{
				s.UnPause();
			}
		}
	}

	public void PlaySound(ESound pKey, AudioSource pSource, bool pIsLoop = false)
	{
		if(pSource == null)
			pSource = AudioSourceNormal;

		AudioClip clip = GetSoundClip(pKey);
		TryPlay(pSource, clip, false, pIsLoop);
	}

	public void PlayWeaponUseSound(EWeaponId pWeapon, AudioSource pSource, bool pIsLoop = false)
	{
		AudioClip clip = GetWeaponUseClip(pWeapon);
		TryPlay(pSource, clip, false, pIsLoop);
	}

	public void PlayMusic(ESound pSound)
	{
		AudioSource source = AudioSourceMusic;
		AudioClip clip = GetSoundClip(pSound);
		TryPlay(source, clip, true, true);
	}

	/// <summary>
	/// All audio source should be started here.
	/// Sets the clip, volume, etc
	/// </summary>
	private void TryPlay(AudioSource source, AudioClip clip, bool pIsMusic, bool pIsLoop)
	{
		if(clip == null)
		{
			Debug.LogError($"Clip is null");
			return;
		}
		if(Brainiacs.Instance.AudioManager.debug_LogSounds)
			Debug.Log($"Play: {clip.name}");

		source.spatialBlend = 1; //set all sources to 3D (dont wanna set it manually)
		
		float musicVolume = debug.MuteMusic ? 0 : playerPrefs.VolumeMusic;
		UpdateAudioVolume(source, pIsMusic ? musicVolume : playerPrefs.VolumeSounds);

		if(pIsMusic) 
			source.Stop();

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
		usedSources.Add(source);
	}

	/// <summary>
	/// Common method for setting audio source volume.
	/// </summary>
	public void UpdateAudioVolume(AudioSource pSource, float pVolume)
	{
		pSource.volume = pVolume;
		pSource.volume /= VOLUME_REDUCTION;
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
