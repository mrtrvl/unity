using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverAnimatorController : MonoBehaviour {
    private Animator diverAnimator;
    private bool move = false;

	void Start () {
        diverAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        diverAnimator.SetBool("move", move); 
    }
}
