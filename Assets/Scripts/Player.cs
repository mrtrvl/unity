using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public Text timeText;
    public Text damageText;
    public Text depthText;
    public Text collectedItemsText;
    public Text healthText;

    public float horizontalSpeed = 1;
    public float baseDepth = 20;
    public float defaultLightRange = 12;
    public float buoyancyFactor = 0.01f;

    private float damage;
    private float damageFactor;
    private Rigidbody2D ridgidbody;

    private GameObject diversLight;
    private bool offTheBottom = true;

    private float depth;
    private float airVolume = 10;

    private int collectedItemsCount = 0;
    private int health = 100;

    private float horizontalMove;
    private float verticalMove;

    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.

    void Start () 
	{
        damage = 0f;
        damageFactor = 0.01f;

        showText();

        ridgidbody = GetComponent<Rigidbody2D>();

        diversLight = GameObject.Find("DiveLamp");
        diversLight.GetComponent<Light>().range = defaultLightRange;
    }

	void Update ()
	{
        depth = baseDepth - ridgidbody.position.y;

        float pressure = depth / 10 + 1;
        float volumeUnderPressure = Mathf.Round(airVolume / pressure * 100) / 100f;
        float buoyancy = volumeUnderPressure - 3.33f;

        //Check if we are running either in the Unity editor or in a standalone build.
        #if UNITY_STANDALONE || UNITY_WEBPLAYER

        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
        #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;

                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;

                touchOrigin.x = -1;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                    horizontalMove = x > 0 ? 1 : -1;
                else
                    verticalMove = y > 0 ? 1 : -1;
            }
        }

        #endif

        ridgidbody.velocity = new Vector2(horizontalMove * horizontalSpeed, buoyancy);
        //ridgidbody.AddForce(new Vector2(0, buoyancy));

        if (verticalMove < 0)
        {
            airVolume -= buoyancyFactor;
        }

        if (verticalMove > 0)
        {
            airVolume += buoyancyFactor;
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

    void OnCollisionExit2D(Collision2D collision)
    {
        offTheBottom = true;
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
            collectedItemsCount += 1;
        }

        if (other.gameObject.CompareTag("Jellyfish"))
        {
            other.gameObject.SetActive(false);
            health -= 5;
        }
    }

    void showText ()
    {
        timeText.text = "Time: " + Time.time.ToString();
        damageText.text = "Damage: " + damage.ToString();
        depthText.text = "Depth: " + depth.ToString();
        collectedItemsText.text = "Collected items: " + collectedItemsCount.ToString();
        healthText.text = "Health: " + health.ToString();
    }
}
