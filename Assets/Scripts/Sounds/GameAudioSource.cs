using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioSource : PoolObject
{
    AudioSource source;

    protected override void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        if(!source.isPlaying)
            ReturnToPool();
    }

    public void Play(AudioClip pClip, Vector2 pPosition)
    {
        transform.position = pPosition;
        source.PlayOneShot(pClip);
    }
}
