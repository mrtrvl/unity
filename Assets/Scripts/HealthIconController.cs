using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace bananaDiver.healthImageController
{
    public class HealthIconController : MonoBehaviour
    {
        public static float amountOfHealth = 100;
        public static bool gotHealth = false;

        private CanvasRenderer textCanvas;
        private CanvasRenderer imageCanvas;
        private bool colorChangedToGreen = false;
        private Color red = new Color(1, 0, 0);
        private Color green = new Color(0, 1, 0);
        private Color white = new Color(1, 1, 1);
        private bool animateHealth = false;

        void Start()
        {
            textCanvas = GameObject.Find("HealthText").GetComponent<CanvasRenderer>();
            imageCanvas = GameObject.Find("HealthImage").GetComponent<CanvasRenderer>();
            SetIconColor(white);
        }

        void Update()
        {
            if (gotHealth)
            {
                SetIconColor(green);
                colorChangedToGreen = true;
                gotHealth = false;
                animateHealth = true;
                imageCanvas.transform.localScale = new Vector3(2, 2, 2);
            }
            else if (amountOfHealth < 50)
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
                iconColor = iconColor + new Color(0.01f, 0, 0.01f);
                SetIconColor(iconColor);
                if (imageCanvas.transform.localScale.x > .7f && animateHealth)
                {
                    imageCanvas.transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
                }
                else
                {
                    imageCanvas.transform.localScale = new Vector3(1, 1, 1);
                    animateHealth = false;
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