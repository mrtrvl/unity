using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTriggerController : MonoBehaviour {

    public GameObject rock;
    public GameObject player;
    public GameObject shatteredRock;
    public GameObject wholeVersion;
    public GameObject TNT;

    //private GameObject shatteredRock;
    private Rigidbody2D rb;
    private bool explosionOccured = false;
    private bool messageBroadcasted = false;
    private float explosionTimer = 5f;
    private bool explosivePlanted = false;
    private int lastTimer = 0;
    private GameObject instantiatedTNT;

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

        if (explosivePlanted)
        {
            if (explosionTimer > 0)
            {
                explosionTimer -= Time.deltaTime;

                if (lastTimer != Mathf.RoundToInt(explosionTimer))
                {
                    showPopUp(Mathf.RoundToInt(explosionTimer).ToString());
                    lastTimer = Mathf.RoundToInt(explosionTimer);
                }
            }
            else if (explosionTimer <= 0)
            {
                explosivePlanted = false;
                Explode();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (hasExplosive && !explosionOccured)
            {
                //Explode();
                explosivePlanted = true;
                explosionOccured = true;
                messageBroadcasted = true;
                instantiatedTNT = (GameObject)Instantiate(TNT, transform.position, transform.rotation);
                //showPopUp("Explosion in " + explosionTimer.ToString() + " seconds...");
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
        Destroy(instantiatedTNT);
    }
}
