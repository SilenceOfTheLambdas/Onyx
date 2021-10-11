using SuperuserUtils;
using System;
using UnityEngine;

public class SoundManager : GenericSingletonClass<SoundManager>
{
    public SoundAudioClip[] soundAudioClipArray;

    [Serializable]
    public enum Sound
    {
        MouseOverSound,
        None
    }
    
    public void PlaySound(SoundManager.Sound sound)
    {
        var soundGameObject = ObjectPooler.Instance.SpawnFromPool("Sound", Vector3.zero, Quaternion.identity);
        var audioSource     = soundGameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
    }

    private AudioClip GetAudioClip(Sound sound)
    {
        foreach (var soundAudioClip in soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound) return soundAudioClip.audioClip;
        }

        Debug.LogError($"Sound: {sound} Not Found!");
        return null;
    }

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip          audioClip;
    }
}