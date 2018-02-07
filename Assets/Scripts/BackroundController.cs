using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackroundController : MonoBehaviour {

    public float parallaxFactor = 1.2f;

    private Camera mainCamera;

    void Start () {
        mainCamera = Camera.main;
    }
	
	void LateUpdate () {

        moveBackground ();
	}

    void moveBackground ()
    {
        transform.position = new Vector3(mainCamera.transform.position.x / parallaxFactor, mainCamera.transform.position.y / parallaxFactor, transform.position.z);
    }
}
