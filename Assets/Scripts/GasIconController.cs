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
        private bool colorChangedToGreen = false;
        private Color red = new Color(1, 0, 0);
        private Color green = new Color(0, 1, 0);
        private Color white = new Color(1, 1, 1);
        private bool animateTank = false;

        void Start()
        {
            textCanvas = GameObject.Find("BreathingGasText").GetComponent<CanvasRenderer>();
            imageCanvas = GameObject.Find("BreathingGasImage").GetComponent<CanvasRenderer>();
            SetIconColor(white);
        }

        void Update()
        {
            if (gotTank)
            {
                SetIconColor(green);
                colorChangedToGreen = true;
                gotTank = false;
                animateTank = true;
                imageCanvas.transform.localScale = new Vector3(2, 2, 2);
            }
            else if (amountOfGas < 100)
            {
                SetIconColor(red);
            }

            if (colorChangedToGreen)
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
                if (imageCanvas.transform.localScale.x > .7f && animateTank)
                {
                    imageCanvas.transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
                }
                else
                {
                    imageCanvas.transform.localScale = new Vector3(1, 1, 1);
                    animateTank = false;
                }
            }
            else
            {
                colorChangedToGreen = false;
            }
        }

        private void SetIconColor(Color color)
        {
            imageCanvas.SetColor(color);
            textCanvas.SetColor(color);
        }

        private void canvasSizeAnimation()
        {
            imageCanvas.transform.localScale = new Vector3(2, 2, 2);
        }
    }
}