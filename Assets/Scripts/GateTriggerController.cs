using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bananaDiver.chestController;

public class GateTriggerController : MonoBehaviour {

    //public GameObject gate;
    //public GameObject player;
    private GameObject player;

    public TextMesh popUp;

    private bool hasKey;

    private Animator gateAmimation;
    private BoxCollider2D gateCollider;

    void Start()
    {
        player = GameObject.Find("Diver");
        gateAmimation = GameObject.Find("Gate").GetComponent<Animator>();
        gateCollider = GameObject.Find("Gate").GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        hasKey = player.GetComponent<Player>().hasKey;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (hasKey)
            {
                //gate.GetComponent<Rigidbody2D>().constraints = Rigidbody2DConstraints.None;
                //gate.transform.position += new Vector3(0, 0, -2);
                //gate.transform.localScale -= new Vector3(0.5F, 0, 0);
                //gate.transform.Rotate(0, 0, 90);
                //gate.SetActive(false);
                ChestController.RemoveItem("Key");
                gateAmimation.SetBool("Open", true);
                gateCollider.enabled = false;
            }
            else
            {
                string message = "You need a key to open the gate!";
                showPopUp(message);
            }
        }
    }
    void showPopUp(string message)
    {
        popUp.text = message;
        Instantiate(popUp, transform.position, transform.rotation);
    }
}
