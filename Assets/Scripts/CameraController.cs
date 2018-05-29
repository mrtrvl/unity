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
        public SpriteRenderer background;

        private new GameObject camera;
        private float defaultCameraZoom;
        private Vector3 centerPosition;
        private Vector3 defaultPosition;
        private float cameraChangeSpeed = 15;
        private float backToStart;
        private Color32 backgroundColor;

        private Vector3 offset;
        private static bool showingMap = false;

        void Start()
        {
            camera = GameObject.Find("Main Camera");
            defaultCameraZoom = camera.GetComponent<Camera>().orthographicSize;
            offset = transform.position - player.transform.position;
            backgroundColor = background.GetComponent<SpriteRenderer>().color;
            backToStart = timeToShowMap + 2;
        }

        void LateUpdate()
        {
            if(!showingMap)
            {
                transform.position = player.transform.position + new Vector3(0, 0, offset.z);
                defaultPosition = transform.position;
            }
            else
            {
                timeToShowMap -= Time.deltaTime;
                if (timeToShowMap <= 0)
                {
                    backToStart -= Time.deltaTime;
                    if (backToStart <= 0)
                    {
                        backround.SetActive(true);
                        camera.GetComponent<Camera>().orthographicSize = defaultCameraZoom;

                        showingMap = false;
                    } else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, offset.z),  Time.deltaTime * cameraChangeSpeed);
                        background.GetComponent<SpriteRenderer>().color += new Color32(0, 0, 0, 1);
                        if (transform.position == new Vector3(player.transform.position.x, player.transform.position.y, offset.z))
                        {
                            if (camera.GetComponent<Camera>().orthographicSize > defaultCameraZoom)
                            {
                                camera.GetComponent<Camera>().orthographicSize -= Time.deltaTime * cameraChangeSpeed;
                            }
                        }
                    }

                }
                else
                {
                    //backround.SetActive(false);
                    centerPosition = backround.GetComponent<SpriteRenderer>().bounds.center;
                    background.GetComponent<SpriteRenderer>().color -= new Color32(0, 0, 0, 1);
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
