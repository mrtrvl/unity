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
    //public float gravityScaleFactor = 0.01f;

    private float damage;
    private float damageFactor;
    private Rigidbody2D ridgidbody;
    private float depth;
    private GameObject diversLight;
    private bool offTheBottom = true;
    private float airVolume = 10;

    //private float diversGravity;
    //private float lastPositionY;

    void Start () 
	{
        damage = 0f;
        damageFactor = 0.01f;
        showText();
        ridgidbody = GetComponent<Rigidbody2D>();
        diversLight = GameObject.Find("DiveLamp");
        diversLight.GetComponent<Light>().range = defaultLightRange;
        //lastPositionY = ridgidbody.transform.position.y;
    }

	void Update ()
	{
        depth = baseDepth - ridgidbody.position.y;

        float pressure = depth / 10 + 1;
        float volumeUnderPressure = Mathf.Round(airVolume / pressure * 100) / 100f;
        float buoyancy = volumeUnderPressure - 3.33f;

        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        ridgidbody.velocity = new Vector2(horizontalMove * horizontalSpeed, ridgidbody.velocity.y);
        ridgidbody.AddForce(new Vector2(0, buoyancy));

        //float currentPositionY = ridgidbody.transform.position.y;
        //float directionY = currentPositionY - lastPositionY;
        //lastPositionY = ridgidbody.transform.position.y;

        if (verticalMove < 0)
        {
            airVolume -= 0.1f;
        }

        if (verticalMove > 0)
        {
            airVolume += 0.1f;
        }

        if (offTheBottom)
        {
            float currentLightRange = diversLight.GetComponent<Light>().range;
            diversLight.GetComponent<Light>().range = Mathf.Lerp(currentLightRange, defaultLightRange, 0.2f * Time.deltaTime);
        }

        showText();
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
