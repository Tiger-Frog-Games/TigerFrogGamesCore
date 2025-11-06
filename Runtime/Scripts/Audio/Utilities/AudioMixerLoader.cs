using System;
using UnityEngine;
using UnityEngine.Audio;

namespace TigerFrogGames.Core
{
    public class AudioMixerLoader : MonoBehaviour
    {
        public static float initialVolume = .5f;
        
        public static String masterVolumeLabel = "MasterVolume";
        public static String musicVolumeLabel = "MusicVolume";
        public static String sfxVolumeLabel = "SFXVolume";
        
        /* ------- Variables ------- */
        [SerializeField] private AudioMixer audioMixer;
       

        /* ------- Unity Methods ------- */

        private void Start()
        {
            float mainLevel = PlayerPrefs.GetFloat("AudioMaster", initialVolume).ToLogarithmicVolume();
            audioMixer.SetFloat(masterVolumeLabel, mainLevel);
            
            float MusicLevel = PlayerPrefs.GetFloat("AudioMusic", initialVolume).ToLogarithmicVolume();
            audioMixer.SetFloat(musicVolumeLabel, MusicLevel);
            
            float SFXLevel = PlayerPrefs.GetFloat("AudioSFX", initialVolume).ToLogarithmicVolume();
            audioMixer.SetFloat(sfxVolumeLabel, SFXLevel);
            
        }
    }
}