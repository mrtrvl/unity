﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTriggerController : MonoBehaviour {

    public GameObject gate;
    public GameObject player;

    public TextMesh popUp;

    private bool hasKey;

    void Start()
    {

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
                gate.transform.position += new Vector3(0, 0, -2);
                gate.transform.localScale -= new Vector3(0.5F, 0, 0);
                gate.transform.Rotate(0, 0, 90);
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