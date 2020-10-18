﻿using System;
using System.Dynamic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using Lean.Transition;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

namespace Actual_Game_Files.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] public Sound[] sounds;
        [Space]
        
        [Header("GameObjects")]
        [SerializeField] public AudioSource musicSource;
        [SerializeField] private Slider globalVolSlider;
        [SerializeField] private Slider pitchSlider;
        [SerializeField] private Slider musicVolSlider;
        [Space]


        public AudioSettingsProfile audioSettingsProfile;
        private string _audioSettingsDirectory;


        private void Awake()
        {
            _audioSettingsDirectory = Application.dataPath + @"\Settings\AudioSettings.json";
            audioSettingsProfile = JsonUtility.FromJson<AudioSettingsProfile>(File.ReadAllText(_audioSettingsDirectory));
            PlayMusic();
        }

        public void WriteAudioSettings()
        {
            audioSettingsProfile.globalVolume = globalVolSlider.value;
            audioSettingsProfile.pitch = pitchSlider.value * 3;
            audioSettingsProfile.musicVolume = musicVolSlider.value;
            File.WriteAllText(_audioSettingsDirectory, JsonUtility.ToJson(audioSettingsProfile));
            PlayMusic();
        }

        public void Play(string nameOfSound, AudioSource desiredAudioSource)
        {
            // ReSharper disable once InconsistentNaming
            var _sound = Array.Find(sounds,sound => sound.soundName == nameOfSound);
            _sound.audioSource = desiredAudioSource;

            if (_sound.audioSource.clip != _sound.audioClip)
                _sound.audioSource.clip = _sound.audioClip;
            
            SetVolumeAndPitch(_sound, desiredAudioSource);
            
            if(_sound.needsPlayOneHit) _sound.audioSource.PlayOneShot(_sound.audioClip);
            else _sound.audioSource.Play();
        }

        private void SetVolumeAndPitch(Sound sound, AudioSource audioSource)
        {
            audioSource.volume = sound.volume * audioSettingsProfile.globalVolume;

            if (sound.soundName == "InGameMusic")
                audioSource.volume *= audioSettingsProfile.musicVolume;
            
            audioSource.pitch = sound.pitch * audioSettingsProfile.pitch;
        }

        private void PlayMusic()
        {
            Play("InGameMusic", musicSource);
        }
    }
}
