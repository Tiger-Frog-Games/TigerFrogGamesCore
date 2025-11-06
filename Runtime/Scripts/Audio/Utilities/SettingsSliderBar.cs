using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TigerFrogGames.Core
{
    public class SettingsSliderBar : MonoBehaviour
    {
        /* ------- Variables ------- */
        [Header("Dependencies")]
       
        [SerializeField] private AudioMixer audioMixer;
        
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [SerializeField] private TMP_Text masterVolumeText;
        [SerializeField] private TMP_Text musicVolumeText;
        [SerializeField] private TMP_Text sfxVolumeText;
        
        
        /* ------- Unity Methods ------- */

        private void Awake()
        {
            float mainLevel = PlayerPrefs.GetFloat("AudioMaster", AudioMixerLoader.initialVolume);
            masterSlider.SetValueWithoutNotify(mainLevel);
            masterSlider.onValueChanged.AddListener(delegate { OnValueChange(masterSlider, "AudioMaster", AudioMixerLoader.masterVolumeLabel, masterVolumeText);});
            masterVolumeText.text = $"{Mathf.FloorToInt(mainLevel * 100)  }";
            
            float MusicLevel = PlayerPrefs.GetFloat("AudioMusic",AudioMixerLoader.initialVolume);
            musicSlider.SetValueWithoutNotify(MusicLevel);
            musicSlider.onValueChanged.AddListener(delegate { OnValueChange(musicSlider, "AudioMusic", AudioMixerLoader.musicVolumeLabel, musicVolumeText);});
            musicVolumeText.text = $"{Mathf.FloorToInt(MusicLevel * 100)  }";
            
            float SFXLevel = PlayerPrefs.GetFloat("AudioSFX", AudioMixerLoader.initialVolume);
            sfxSlider.SetValueWithoutNotify(SFXLevel);
            sfxSlider.onValueChanged.AddListener(delegate { OnValueChange(sfxSlider, "AudioSFX", AudioMixerLoader.sfxVolumeLabel, sfxVolumeText);});
            sfxVolumeText.text = $"{Mathf.FloorToInt(SFXLevel * 100)}";
        }

        private void OnValueChange(Slider slider, string playerPrefsLabel, string mixerLabel, TMP_Text volumeText)
        {
            PlayerPrefs.SetFloat(playerPrefsLabel, slider.value);
            audioMixer.SetFloat(mixerLabel, slider.value.ToLogarithmicVolume());
            volumeText.text = $"{Mathf.FloorToInt(slider.value * 100) }";
        }
        
    }
}