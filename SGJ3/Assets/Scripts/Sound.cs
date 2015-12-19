using UnityEngine;
using System.Collections;

public class Sound : Singleton<Sound>
{
    protected override void Init()
    {
    }

    public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 0.8f, float pitch = 1f, ulong delay = 0)
    {
        if(!clip) return null;
        
        var source = CreateAudioSource(position, clip.name);
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 1f;
        source.minDistance = 5f;
        source.maxDistance = 1000f;
        source.Play(delay);
        Object.Destroy(source.gameObject, clip.length);
        return source;
    }

    private AudioSource CreateAudioSource(Vector3 position, string clipName)
    {
        var go = new GameObject("Audio " + clipName);
        go.transform.position = position;
        return go.AddComponent<AudioSource>();
    }
}
