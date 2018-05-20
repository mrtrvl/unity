using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bananaDiver.chestController;

public class RockTriggerController : MonoBehaviour {

    public GameObject rock;
    //public GameObject player;
    public GameObject shatteredRock;
    public GameObject wholeVersion;
    public GameObject TNT;

    //private GameObject shatteredRock;
    private Rigidbody2D rb;
    private bool explosionOccured;
    private bool messageBroadcasted;
    private float explosionTimer;
    private bool explosivePlanted;
    private int lastTimer;
    private GameObject instantiatedTNT;

    public TextMesh popUp;

    private bool hasExplosive;

    void Start()
    {
        //shatteredRock = shatteredVersion.gameObject;
        //rb = GetComponent<Rigidbody2D>();
        SetParameters();
    }

    void Update()
    {
        //hasExplosive = player.GetComponent<Player>().hasExplosive;

        if (explosivePlanted)
        {
            if (explosionTimer > 0)
            {
                explosionTimer -= Time.deltaTime;
                int seconds = System.Convert.ToInt32(explosionTimer % 60);

                if (lastTimer != seconds)
                {
                    ShowPopUp(Mathf.RoundToInt(explosionTimer).ToString());
                    lastTimer = seconds;
                }
            }
            else if (explosionTimer <= 0)
            {
                SetParameters();
                Explode();
            }
        }

        
    }

    void SetParameters()
    {
        explosionOccured = false;
        messageBroadcasted = false;
        explosionTimer = 7f;
        explosivePlanted = false;
        lastTimer = (int)explosionTimer;
}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(ItemTag.Player))
        {
            hasExplosive = ChestController.DoesHaveItem(ItemTag.Tnt);

            if (hasExplosive && !explosionOccured)
            {
                //Explode();
                explosivePlanted = true;
                explosionOccured = true;
                messageBroadcasted = true;
                instantiatedTNT = (GameObject)Instantiate(TNT, new Vector3(transform.position.x + 1.5f, transform.position.y, transform.position.z), transform.rotation);
                instantiatedTNT.tag = "Untagged";
                //showPopUp("Explosion in " + explosionTimer.ToString() + " seconds...");
                ChestController.RemoveItem(ItemTag.Tnt);
            }
            else if(!messageBroadcasted)
            {
                string message = "You need a TNT to destroy this rock!";
                messageBroadcasted = true;
                ShowPopUp(message);
            }
        }
    }

    void ShowPopUp(string message)
    {
        popUp.text = message;
        Instantiate(popUp, new Vector3(transform.position.x + 1.5f, transform.position.y, -1), transform.rotation);
    }

    void Explode()
    {
        messageBroadcasted = true;
        Instantiate(shatteredRock, transform.position, transform.rotation);
        Destroy(wholeVersion.gameObject);
        Destroy(instantiatedTNT);
    }
}
