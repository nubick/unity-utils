using System;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu]
    public class SoundEffect : ScriptableObject
    {
        private int _nextAudioClipIndex;

        public SoundEffectType Type;
        public AudioClip AudioClip;
        public AudioClip[] AudioClips;
        public SoundEffectSelectionType SelectionType;
        public float Volume = 1f;

        public void Play()
        {
            //Commands.PlaySoundEffect.Publish(this);
        }

        public void Play(AudioSource audioSource)
        {
            switch (Type)
            {
                case SoundEffectType.Single:
                    audioSource.PlayOneShot(AudioClip, Volume);
                    break;
                case SoundEffectType.Multiple:
                    PlayMultiple(audioSource);
                    break;
                default:
                    throw new Exception("Not supported SoundEffectType: " + Type);
            }
        }

        private void PlayMultiple(AudioSource audioSource)
        {
            switch (SelectionType)
            {
                case SoundEffectSelectionType.Random:
                    AudioClip randomAudioClip = RandomUtil.NextItem(AudioClips);
                    audioSource.PlayOneShot(randomAudioClip, Volume);
                    break;
                case SoundEffectSelectionType.Sequential:
                    AudioClip nextAudioClip = AudioClips[_nextAudioClipIndex % AudioClips.Length];
                    _nextAudioClipIndex++;
                    audioSource.PlayOneShot(nextAudioClip, Volume);
                    break;
                default:
                    throw new Exception("Not supported SelectionType: " + SelectionType);
            }
        }
    }

    public enum SoundEffectType
    {
        Single,
        Multiple
    }

    public enum SoundEffectSelectionType
    {
        Random,
        Sequential
    }
}