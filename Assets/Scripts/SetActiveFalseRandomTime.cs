using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveFalseRandomTime : MonoBehaviour {

    private float timeToSetActiveFalse;
    private float startTime;

    void Start () {
        timeToSetActiveFalse = Random.Range(2, 5);
        startTime = Time.time;
    }
	
	void Update () {

        float timeToCompare = Time.time - startTime;

		if (timeToCompare > timeToSetActiveFalse)
        {
            gameObject.SetActive(false);
        }
	}
}
