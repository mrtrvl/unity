using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour {

    public GameObject ceilingPiece;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        ceilingPiece.GetComponent<Rigidbody2D>().gravityScale = 0.2f;
        ceilingPiece.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
    }
}
