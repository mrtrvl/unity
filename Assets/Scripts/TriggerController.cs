using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour {

    public GameObject ceilingPiece;

	void Start () {
		
	}

    void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        ceilingPiece.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
    }
}
