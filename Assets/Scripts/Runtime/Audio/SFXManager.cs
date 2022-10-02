using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Dan
{
    public class SFXManager : MonoBehaviour
    {
        private static SFXManager _obj;
        
        private static AudioSource[] _audioSources;
        
        private static Dictionary<string, AudioClip> _audioClipDictionary;
        
        [SerializeField] private int audioChannels = 50;
        [SerializeField] private AudioMixerGroup _sfxAudioMixerGroup;
        [SerializeField] private AudioClip[] audioClips;

        private void Awake()
        {
            _obj = this;
            _audioClipDictionary = new Dictionary<string, AudioClip>();
            _audioSources = new AudioSource[audioChannels];

            for (var i = 0; i < audioChannels; i++)
            {
                var newAudioSource = new GameObject("Audio Channel " + i, typeof(AudioSource))
                {
                    isStatic = true,
                    transform =
                    {
                        parent = transform
                    }
                };
                _audioSources[i] = newAudioSource.GetComponent<AudioSource>();
                _audioSources[i].outputAudioMixerGroup = _sfxAudioMixerGroup;
            }

            foreach (var audioClip in audioClips)
            {
                _audioClipDictionary.Add(audioClip.name, audioClip);
            }
        }

        /// <summary>
        /// This function finds the Audio Source that is not being used right now and returns its index.
        /// </summary>
        /// <returns></returns>
        private static int GetNextFreeAudioSource()
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                if (_audioSources[i].isPlaying) continue;
                return i;
            }

            return 0;
        }

        public static void Play(string soundName)
        {
            if (!_audioClipDictionary.ContainsKey(soundName))
            {
                Debug.LogError("Audio clip with name " + soundName + " does not exist.");
                return;
            }
            var audioSource = _audioSources[GetNextFreeAudioSource()];
            audioSource.outputAudioMixerGroup = _obj._sfxAudioMixerGroup;
            audioSource.PlayOneShot(_audioClipDictionary[soundName]);
        }

        public static AudioSource Play(AudioClip audioClip)
        {
            var audioSource = _audioSources[GetNextFreeAudioSource()];
            audioSource.outputAudioMixerGroup = _obj._sfxAudioMixerGroup;
            audioSource.PlayOneShot(audioClip);
            return audioSource;
        }

        public static void PlayDelayed(string soundName, float delay, bool loop = false) => _obj.StartCoroutine(PlayDelayedSound(_audioClipDictionary[soundName], delay, loop));
        
        public static void PlayDelayed(AudioClip audioClip, float delay, bool loop = false) => _obj.StartCoroutine(PlayDelayedSound(audioClip, delay, loop));

        private static IEnumerator PlayDelayedSound(AudioClip audioClip, float delay, bool loop)
        {
            yield return new WaitForSeconds(delay);
            if (loop)
            {
                var audioSource = _audioSources[GetNextFreeAudioSource()];
                audioSource.clip = audioClip;
                audioSource.loop = true;
                audioSource.Play((ulong)delay);
                yield break;
            }
            Play(audioClip);
        }
    }
}