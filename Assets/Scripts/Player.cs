using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public Text timeText;
    public Text damageText;
    public Text depthText;
    public Text collectedItemsText;
    public Text healthText;

    public float horizontalSpeed = 1f;
    public float baseDepth = 20f;
    public float defaultLightRange = 12f;
    public float minimumLightRange = 7f;
    public float buoyancyFactor = 0.01f;

    private float damage;
    private float damageFactor;
    private Rigidbody2D ridgidbody;

    private GameObject diversLight;
    private bool offTheBottom = true;

    private float depth;
    private float airVolume = 10f;

    private int collectedItemsCount = 0;
    private int health = 100;

    private float horizontalMove;
    private float verticalMove;

    private Vector3 oldPosition;
    private Vector3 newPosition;

    private Vector3 oldLampDirection = new Vector3(1, 0, 0);
    private Quaternion rotation;

    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.

    void Start () 
	{
        damage = 0f;
        damageFactor = 0.01f;

        showText();

        ridgidbody = GetComponent<Rigidbody2D>();

        diversLight = GameObject.Find("DiveLamp");
        diversLight.GetComponent<Light>().range = defaultLightRange;

        oldPosition = ridgidbody.transform.position;
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
                    verticalMove = y > 0 ? 0.5f : -0.5f;
            }
        }

        horizontalMove = Mathf.MoveTowards(horizontalMove, 0, 0.01f);
        verticalMove = Mathf.MoveTowards(verticalMove, 0, 0.01f);

        #endif

        ridgidbody.velocity = new Vector2(ridgidbody.velocity.x, buoyancy);
        ridgidbody.AddForce(new Vector2(horizontalMove, 0));

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

        newPosition = ridgidbody.transform.position;

        showText();

        rotateLight();
    }

    void OnCollisionStay2D (Collision2D collision)
    {
        if (collision.gameObject.name == "Roof")
        {
            damage += damageFactor;
        }

        if(collision.gameObject.name == "Bottom" && diversLight.GetComponent<Light>().range > minimumLightRange)
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

    void rotateLight()
    {
        Vector3 direction = (newPosition - oldPosition);
        if (direction != new Vector3(0, 0, 0))
        {
            rotation = Quaternion.LookRotation(direction);
            oldLampDirection = direction;
        }
        else
        {
            rotation = Quaternion.LookRotation(oldLampDirection);
        }

        diversLight.transform.rotation = rotation;

        oldPosition = ridgidbody.transform.position;
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
