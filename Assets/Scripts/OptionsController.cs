using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace bananaDiver.optionsController
{
    public class OptionsController : MonoBehaviour
    {
        private Slider soundSlider;
        private Slider musicSlider;
        public static float soundSliderValue;
        public static float musicSliderValue;

        private void Start()
        {
            soundSlider = GameObject.Find("SoundSlider").GetComponent<Slider>();
            musicSlider = GameObject.Find("BGMusic").GetComponent<Slider>();
            var gameOptions = GameController.gameController.LoadOptions();
            if (gameOptions != null)
            {
                soundSlider.value = gameOptions.Sound;
                musicSlider.value = gameOptions.Music;
            }
            musicSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
            soundSlider.onValueChanged.AddListener(delegate { ValueChangeCheckForSound(); });
        }

        private void Update()
        {
            soundSliderValue = soundSlider.value;
            musicSliderValue = musicSlider.value;
        }

        private void ValueChangeCheck()
        {
            AudioManager.audioManager.ChangeCurrentlyPlayingSoundVolume(AudioFile.Main, musicSlider.value, true);
        }

        private void ValueChangeCheckForSound()
        {
            AudioManager.audioManager.ChangeSoundVolumeInOptionsByPlayingSound(AudioFile.BreathingWithBubbles, soundSlider.value);
        }
    }
}
