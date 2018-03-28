using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bananaDiver.optionsController;

public class BackroundController : MonoBehaviour {

    public float parallaxFactor = 1.2f;

    private Camera mainCamera;
    private AudioSource backroundMusic;

    void Start () {
        mainCamera = Camera.main;
        backroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
        backroundMusic.volume = OptionsController.musicSliderValue;
    }
	
	void LateUpdate () {

        moveBackground ();
	}

    void moveBackground ()
    {
        transform.position = new Vector3(mainCamera.transform.position.x / parallaxFactor, mainCamera.transform.position.y / parallaxFactor, transform.position.z);
    }
}
