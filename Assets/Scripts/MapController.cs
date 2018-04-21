using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bananaDiver.mapController
{
    public class MapController : MonoBehaviour
    {

        public Texture2D MapImage;
        public GameObject player;

        private static bool shown = false;
        private static bool isAppearing = false;
        private float timer = 2f;
        private static bool hasMap;

        void Start()
        {
        }

        void Update()
        {
            hasMap = player.GetComponent<Player>().hasMap;

            if (isAppearing)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    isAppearing = false;
                    shown = true;
                    gameObject.SetActive(false);
                }
            }
        }

        public static void showMap ()
        {
            if (!shown)
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
