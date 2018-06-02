using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bananaDiver.cameraController
{
    public class CameraController : MonoBehaviour
    {

        public GameObject player;
        public GameObject backround;
        public float zoom;
        public float timeToShowMap;

        private GameObject camera;
        private float defaultCameraZoom;
        private Vector3 centerPosition;
        private float cameraChangeSpeed = 15;

        private Vector3 offset;
        private static bool showingMap = false;

        void Start()
        {
            camera = GameObject.Find("Main Camera");
            defaultCameraZoom = camera.GetComponent<Camera>().orthographicSize;
            offset = transform.position - player.transform.position;
        }

        void LateUpdate()
        {
            if(!showingMap)
            {
                transform.position = player.transform.position + new Vector3(0, 0, offset.z);
            }
            else
            {
                timeToShowMap -= Time.deltaTime;
                if (timeToShowMap <= 0)
                {
                    backround.SetActive(true);
                    camera.GetComponent<Camera>().orthographicSize = defaultCameraZoom;
                    showingMap = false;
                }
                else
                {
                    backround.SetActive(false);
                    centerPosition = backround.GetComponent<SpriteRenderer>().bounds.center;
                    Debug.Log("wut");
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(centerPosition.x, centerPosition.y, transform.position.z), Time.deltaTime * cameraChangeSpeed);
                    if (camera.GetComponent<Camera>().orthographicSize < zoom)
                    {
                        camera.GetComponent<Camera>().orthographicSize += Time.deltaTime * cameraChangeSpeed;
                    }
                }
            }
        }

        public static void ShowMap()
        {
            showingMap = true;
        }
    }
}
