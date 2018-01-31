using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public Text timeText;
    public Text damageText;
    public Text depthText;

    public float horizontalSpeed = 1;
    public float baseDepth = 20;
    public float defaultLightRange = 12;
    public float gravityScaleFactor = 0.01f;

    private float damage;
    private float damageFactor;
    private Rigidbody2D ridgidbody;
    private float depth;
    private GameObject diversLight;
    private bool offTheBottom = true;
    private float diversGravity;
    private float lastPositionY;

    void Start () 
	{
        damage = 0f;
        damageFactor = 0.01f;
        showText();
        ridgidbody = GetComponent<Rigidbody2D>();
        diversLight = GameObject.Find("DiveLamp");
        diversLight.GetComponent<Light>().range = defaultLightRange;
        lastPositionY = ridgidbody.transform.position.y;
    }

	void Update ()
	{
        showText();
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        ridgidbody.velocity = new Vector2(horizontalMove * horizontalSpeed, ridgidbody.velocity.y);

        depth = baseDepth - ridgidbody.position.y;

        float currentPositionY = ridgidbody.transform.position.y;
        float directionY = currentPositionY - lastPositionY;
        lastPositionY = ridgidbody.transform.position.y;

        if ((verticalMove < 0) || (directionY < 0))
        {
            ridgidbody.gravityScale += gravityScaleFactor;
            
            if (ridgidbody.gravityScale > 1)
            {
                ridgidbody.gravityScale = 1;
            }
        }

        if ((verticalMove > 0) || (directionY > 0))
        {
            ridgidbody.gravityScale -= gravityScaleFactor;

            if (ridgidbody.gravityScale < -1)
            {
                ridgidbody.gravityScale = 1;
            }
        }

        if (offTheBottom)
        {
            float currentLightRange = diversLight.GetComponent<Light>().range;
            diversLight.GetComponent<Light>().range = Mathf.Lerp(currentLightRange, defaultLightRange, 0.2f * Time.deltaTime);
        }
    }

    void OnCollisionStay2D (Collision2D collision)
    {
        if (collision.gameObject.name == "Roof")
        {
            damage += damageFactor;
        }

        if(collision.gameObject.name == "Bottom")
        {
            diversLight.GetComponent<Light>().range -= 0.1f;
            offTheBottom = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        offTheBottom = true;
    }

    void showText ()
    {
        timeText.text = "Time: " + Time.time.ToString();
        damageText.text = "Damage: " + damage.ToString();
        depthText.text = "Depth: " + depth.ToString();
    }
}
