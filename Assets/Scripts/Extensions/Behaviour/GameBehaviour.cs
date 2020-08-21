using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : BrainiacsBehaviour
{
	protected Game game => Game.Instance;

	private Dictionary<ESound, float> lastPlaySoundTime = new Dictionary<ESound, float>();

	public void PlaySound(ESound pKey, float pMaxFrequency = 0, bool pIsLoop = false)
	{
		float lastPlayTime;
		bool keyExists = lastPlaySoundTime.TryGetValue(pKey, out lastPlayTime);
		if(Time.time - lastPlayTime < pMaxFrequency)
		{
			//Debug.Log($"Cant play sound {pKey}, frequency = {pMaxFrequency}");
			return;
		}
		if(keyExists)
			lastPlaySoundTime[pKey] = Time.time;
		else
			lastPlaySoundTime.Add(pKey, Time.time);

		AudioSource source = InstanceFactory.Instantiate(game.AudioSourcePrefab.gameObject).GetComponent<AudioSource>();
		source.gameObject.SetActive(true);
		SoundController.PlaySound(pKey, source, pIsLoop);
	}

}
