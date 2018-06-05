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
        private float cameraChangeSpeed = 15;
        private float backToStart;

        private Vector3 offset;
        private static bool showingMap = false;
        private Color32 defaultBackgroundColor;

        void Start()
        {
            camera = GameObject.Find("Main Camera");
            defaultCameraZoom = camera.GetComponent<Camera>().orthographicSize;
            offset = transform.position - player.transform.position;
            backToStart = timeToShowMap + 2;
            defaultBackgroundColor = background.GetComponent<SpriteRenderer>().color;
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
                    backToStart -= Time.deltaTime;
                    if (backToStart <= 0)
                    {
                        camera.GetComponent<Camera>().orthographicSize = defaultCameraZoom;
                        showingMap = false;
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, offset.z),  Time.deltaTime * cameraChangeSpeed);
                        background.GetComponent<SpriteRenderer>().color += new Color32(0, 0, 0, 1);
                        if (transform.position == new Vector3(player.transform.position.x, player.transform.position.y, offset.z))
                        {
                            if (camera.GetComponent<Camera>().orthographicSize > defaultCameraZoom)
                            {
                                camera.GetComponent<Camera>().orthographicSize -= Time.deltaTime * cameraChangeSpeed;
                            }
                            else
                            {
                                background.GetComponent<SpriteRenderer>().color = defaultBackgroundColor;
                                showingMap = false;
                            }
                        }
                    }

                }
                else
                {
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
