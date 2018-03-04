using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour {

    public GameObject shatteredVersion;

    private GameObject shatteredRock;
    private Rigidbody2D rb;

	void Start () {
        shatteredRock = shatteredVersion.gameObject;
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Explode()
    {
        Instantiate(shatteredVersion, transform.position, transform.rotation);
        rb.AddForce(transform.up * 2000); // Ei tööta hetkel... Peab ehk igale childile eraldi jõudu avaldama?
        Destroy(gameObject);
    }
}
