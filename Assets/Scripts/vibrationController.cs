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

        private void Start()
        {
            onButton = GameObject.Find("VibrationOn").GetComponent<Button>();
            offButton = GameObject.Find("VibrationOff").GetComponent<Button>();
            onButton.onClick.AddListener(toggle);
            offButton.onClick.AddListener(toggle);
            vibrationOn = GameController.gameController.LoadOptions().VibrationOn;;
            setButtonImage();
        }

        private void Update()
        {
        }

        private void toggle()
        {
            switchOn = !switchOn;
            switchOff = !switchOff;
            vibrationOn = !vibrationOn;
            setButtonImage();
        }

        private void setButtonImage()
        {
            if (vibrationOn)
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
