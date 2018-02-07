using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackround : MonoBehaviour {

    private Camera mainCamera;

    void Start () {
        mainCamera = Camera.main;
    }
	
	void Update () {

        transform.position = new Vector3(mainCamera.transform.position.x / 1.2f, mainCamera.transform.position.y / 1.2f, 0.1f);
	}
}
