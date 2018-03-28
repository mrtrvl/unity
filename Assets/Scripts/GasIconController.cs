using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace bananaDiver.gasImageController
{
    public class GasIconController : MonoBehaviour
    {
        public static float amountOfGas = 150;
        public static bool gotTank = false;

        private CanvasRenderer textCanvas;
        private CanvasRenderer imageCanvas;
        private bool colorChangingToGreen = false;
        private Color red = new Color(1, 0, 0);
        private Color green = new Color(0, 1, 0);
        private Color white = new Color(1, 1, 1);

        void Start()
        {
            //image = gameObject.GetComponent<Image>();
            textCanvas = GameObject.Find("BreathingGasText").GetComponent<CanvasRenderer>();
            imageCanvas = GameObject.Find("BreathingGasImage").GetComponent<CanvasRenderer>();
            SetIconColor(white);
        }

        // Update is called once per frame
        void Update()
        {
            if (gotTank)
            {
                SetIconColor(green);
                colorChangingToGreen = true;
                gotTank = false;
            }
            else if (amountOfGas < 100)
            {
                SetIconColor(red);
            }

            if (colorChangingToGreen)
            {
                IconColorGreenToWhite();
            }
        }

        private void IconColorGreenToWhite()
        {
            if (imageCanvas.GetColor() != white)
            {
                Color iconColor = imageCanvas.GetColor();
                iconColor = iconColor + new Color(0.002f, 0, 0.002f);
                SetIconColor(iconColor);
            }
            else
            {
                colorChangingToGreen = false;
            }
        }

        private void SetIconColor(Color color)
        {
            imageCanvas.SetColor(color);
            textCanvas.SetColor(color);
        }
    }
}