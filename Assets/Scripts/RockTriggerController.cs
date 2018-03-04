using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTriggerController : MonoBehaviour {

    public GameObject rock;
    public GameObject player;
    public GameObject shatteredRock;
    public GameObject wholeVersion;

    //private GameObject shatteredRock;
    private Rigidbody2D rb;
    private bool explosionOccured = false;
    private bool messageBroadcasted = false;

    public TextMesh popUp;

    private bool hasExplosive;

    void Start()
    {
        //shatteredRock = shatteredVersion.gameObject;
        //rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        hasExplosive = player.GetComponent<Player>().hasExplosive;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (hasExplosive && !explosionOccured)
            {
               Explode();
                explosionOccured = true;
                messageBroadcasted = true;
            }
            else if(!messageBroadcasted)
            {
                string message = "You need a TNT to destroy this rock!";
                messageBroadcasted = true;
                showPopUp(message);
            }
        }
    }

    void showPopUp(string message)
    {
        Handheld.Vibrate();
        popUp.text = message;
        Instantiate(popUp, transform.position, transform.rotation);
    }

    void Explode()
    {
        Instantiate(shatteredRock, transform.position, transform.rotation);
        //rb.AddForce(transform.up * 2000); // Ei tööta hetkel... Peab ehk igale childile eraldi jõudu avaldama?
        Destroy(wholeVersion.gameObject);
    }
}
