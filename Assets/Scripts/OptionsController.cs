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

        void Start()
        {
            soundSlider = GameObject.Find("SoundSlider").GetComponent<Slider>();
            musicSlider = GameObject.Find("BGMusic").GetComponent<Slider>();
        }

        void Update()
        {
            soundSliderValue = soundSlider.value;
            musicSliderValue = musicSlider.value;
        }
    }
}
