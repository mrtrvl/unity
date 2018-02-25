using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpController : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(0, Time.deltaTime, 0);
        if (transform.position.y >= 6)
        {
            Destroy(gameObject);
        }
	}
}
