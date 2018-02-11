using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesController : MonoBehaviour {

    public ParticleSystem bubbles;
    private AudioSource bubbleAudio;
    private bool playAudio;

    // Use this for initialization
    void Start () {
        bubbleAudio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(bubbles.particleCount);
        if (playAudio)
        {
            bubbleAudio.Play();
            playAudio = false;
        } 

        if (bubbles.particleCount < 10 && !bubbleAudio.isPlaying)
        {
            //playAudio = true;
        }
	}
}
