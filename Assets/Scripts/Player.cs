﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public Text timeText;
    public Text damageText;
    public float horizontalSpeed = 1;

    private float damage;
    private float damageFactor;
    private Light diversLight;
    private float lightRange;
    private Rigidbody2D ridgidbody;

	void Start () 
	{
        damage = 0f;
        damageFactor = 0.01f;
        showText();
        ridgidbody = GetComponent<Rigidbody2D>();
    }

	void Update ()
	{
        showText();
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        ridgidbody.velocity = new Vector2(horizontalMove * horizontalSpeed, verticalMove);
    }

    void OnCollisionStay2D (Collision2D collision)
    {
        if (collision.gameObject.name == "Roof")
        {
            damage += damageFactor;
        }

        if(collision.gameObject.name == "Bottom")
        {
            //diversLight.range -= 0.1f;
        }
    }

    void showText ()
    {
        timeText.text = "Time: " + Time.time.ToString();
        damageText.text = "Damage: " + damage.ToString();
    }
}
