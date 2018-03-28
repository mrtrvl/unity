using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace bananaDiver.vibrationController
{
    public class vibrationController : MonoBehaviour
    {

        private bool switchOn = true;
        private bool switchOff = false;
        private Button onButton;
        private Button offButton;

        public static bool vibrationOn = true;
        public Sprite activeOnImage;
        public Sprite inactiveOnImage;
        public Sprite activeOffImage;
        public Sprite inactiveOffImage;

        void Start()
        {
            onButton = GameObject.Find("VibrationOn").GetComponent<Button>();
            offButton = GameObject.Find("VibrationOff").GetComponent<Button>();
            onButton.onClick.AddListener(toggle);
            offButton.onClick.AddListener(toggle);
        }

        void Update()
        {

        }

        void toggle()
        {
            switchOn = !switchOn;
            switchOff = !switchOff;
            vibrationOn = !vibrationOn;
            setButtonImage();
        }

        void setButtonImage()
        {
            if (switchOn)
            {
                onButton.image.sprite = activeOnImage;
                offButton.image.sprite = inactiveOffImage;
            }
            else
            {
                onButton.image.sprite = inactiveOnImage;
                offButton.image.sprite = activeOffImage;
            }
        }
    }
}
