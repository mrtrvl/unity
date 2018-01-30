using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public Text timeText;
    public Text damageText;
    public Text depthText;
    public float horizontalSpeed = 1;
    public float baseDepth = 20;

    private float damage;
    private float damageFactor;
    private Rigidbody2D ridgidbody;
    public float depth;

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

        depth = baseDepth - ridgidbody.position.y;
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
        depthText.text = "Depth: " + depth.ToString();
    }
}
