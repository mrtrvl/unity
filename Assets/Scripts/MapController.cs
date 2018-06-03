using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bananaDiver.chestController;

namespace bananaDiver.mapController
{
    public class MapController : MonoBehaviour
    {

        public Texture2D MapImage;

        //private static bool shown = false;
        private static bool isAppearing = false;
        private float timer = 2f;

        void Start()
        {
        }

        void Update()
        {
            if (isAppearing)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    isAppearing = false;
                    //shown = true;
                    gameObject.SetActive(false);
                }
            }
        }

        public static void showMap ()
        {
            if (ChestController.DoesHaveItem(ItemTag.Map))
            {
                isAppearing = true;
            }
        }

        void OnGUI()
        {
            if (isAppearing)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MapImage);
            }
        }
    }

}
