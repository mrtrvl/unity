using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bananaDiver.vibrationController;

public class PopUpController : MonoBehaviour {
    bool vibrationOn;

	// Use this for initialization
	void Start () {
        bool vibrationOn = vibrationController.vibrationOn;
        vibration();
    }
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(0, Time.deltaTime, 0);
        if (transform.position.y >= 6)
        {
            Destroy(gameObject);
        }
	}

    void vibration()
    {
    #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

            if (vibrationOn)
            {
                Handheld.Vibrate();
            }
    #endif
    }
}
