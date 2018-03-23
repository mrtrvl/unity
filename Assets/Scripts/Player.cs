using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    public Text timeText;
    public Text damageText;
    public Text depthText;
    public Text collectedItemsText;
    public Text healthText;
    public Text breathingGasText;
    public Text buoyancyText;

    public float horizontalSpeed = 1f;
    public float baseDepth = 20f;
    public float defaultLightRange = 12f;
    public float minimumLightRange = 7f;
    public float buoyancyFactor = 0.01f;

    public AudioSource screamAudio;

    public GameObject pickUpsPanel;
    public GameObject bubbles;

    public bool hasKey = false;
    public bool hasMap = false;
    public bool hasExplosive = false;
    public bool hasBanana = false;

    public TextMesh popUp;

    public string nextLevel;

    private float damage = 0f;
    private float damageFactor = 0.01f;
    private Rigidbody2D ridgidbody;

    private GameObject diversLight;
    private bool offTheBottom = true;

    private GameObject diverSprite;
    private Animator diversAnimation;

    private float depth;
    private float airVolume = 10f;

    private int collectedItemsCount = 0;
    private int health = 100;

    private float horizontalMove;
    private float verticalMove;

    private float breathingGasAmount = 300;

    private Vector3 oldPosition;
    private Vector3 newPosition;

    private Vector3 oldLampDirection = new Vector3(90, 0, 0);
    private Quaternion rotation;

    private int healthIncreaseStep = 15;
    private int healthDecreaseStep = 5;
    private int breathingGasIncreaseStep = 200;

    private float timeToNextBreath = 0;
    private GameObject bubblesObject;

    private const float breathInterval = 10.0f;

    void Start () 
	{
        screamAudio.Stop();

        ridgidbody = GetComponent<Rigidbody2D>();

        diversLight = GameObject.Find("DiveLamp");
        diversLight.GetComponent<Light>().range = defaultLightRange;

        diverSprite = GameObject.Find("sukelduja");
        diversAnimation = diverSprite.GetComponent<Animator>();

        oldPosition = ridgidbody.transform.position;

        ManageBubbles();

        showText();
    }

	void Update ()
	{


        //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER

        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        horizontalMove = CrossPlatformInputManager.GetAxis("Horizontal");
        verticalMove = CrossPlatformInputManager.GetAxis("Vertical");
#endif

        float buoyancy = calculateBuoyancy();

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

        manageBreathingGas();

        flipToMoveDirection();

        showText();

        controlLight();

        ManageBubbles();

        manageAnimation();
    }

    void ManageBubbles()
    {
        if (Time.time >= timeToNextBreath)
        {
            if (bubblesObject != null)
            {
                Destroy(bubblesObject);
            }
            timeToNextBreath = Time.time + breathInterval;
            bubblesObject = Instantiate(bubbles, gameObject.transform);
        }
        
    }

    void manageAnimation()
    {
        if (horizontalMove > 0.1 || horizontalMove < -0.1)
        {
            diversAnimation.SetBool("move", true);
        }
        else
        {
            diversAnimation.SetBool("move", false);
        }
    }

    void flipToMoveDirection()
    {
        if (ridgidbody.velocity.x > 0.1f)
        {
            transform.localScale = new Vector2(-0.5f, transform.localScale.y);
        }

        else if (ridgidbody.velocity.x < -0.1f)
        {
            transform.localScale = new Vector2(0.5f, transform.localScale.y);
        }
    }

    float calculateBuoyancy()
    {
        depth = baseDepth - ridgidbody.position.y;

        float pressure = depth / 10 + 1; // 1 atm per 10 m + 1 atm on surface
        float volumeUnderPressure = Mathf.Round(airVolume / pressure * 100) / 100f;
        float buoyancy = volumeUnderPressure - 3.33f;

        return buoyancy;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jellyfish"))
        {
            health -= healthDecreaseStep;
            screamAudio.Play();
            string message = "Health -" + healthDecreaseStep.ToString();
            showPopUp(message);
        }
    }

    void OnCollisionStay2D (Collision2D collision)
    {
        if (collision.gameObject.name == "Ceiling")
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
        if (other.gameObject.CompareTag("Coin") || other.gameObject.CompareTag("Emerald") || other.gameObject.CompareTag("Diamond"))
        {
            collectedItemsCount += 1;
            string message = "You got something valuable!";
            other.gameObject.SetActive(false);
            showPopUp(message);
        }
        else if (other.gameObject.CompareTag("Key"))
        {
            hasKey = true;
            other.transform.SetParent(pickUpsPanel.transform);
            other.transform.localPosition = new Vector3(0, 0, 0);

            var pointLight = other.transform.Find("Point light");
            pointLight.gameObject.SetActive(false);

            string message = ("You got a key!");
            showPopUp(message);
        }
        else if (other.gameObject.CompareTag("Medkit"))
        {
            health += healthIncreaseStep;
            other.gameObject.SetActive(false);
            string message = "Health +" + healthIncreaseStep.ToString();
            showPopUp(message);
        }
        else if (other.gameObject.CompareTag("Tank"))
        {
            breathingGasAmount += breathingGasIncreaseStep;
            string message = "Breathing gas +" + breathingGasIncreaseStep.ToString();
            showPopUp(message);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("TNT"))
        {
            hasExplosive = true;
            other.transform.SetParent(pickUpsPanel.transform);

            var pointLight = other.transform.Find("Point light");
            pointLight.gameObject.SetActive(false);

            other.transform.localPosition = new Vector3(-1, 0, 0);
            string message = "You got a TNT!";
            showPopUp(message);
        }
        else if (other.gameObject.CompareTag("Map"))
        {
            hasMap = true;
            other.transform.SetParent(pickUpsPanel.transform);
            other.transform.localPosition = new Vector3(-2, 0, 0);

            var pointLight = other.transform.Find("Point light");
            pointLight.gameObject.SetActive(false); 

            string message = "You got a Map!";
            showPopUp(message);
        }
        else if (other.gameObject.CompareTag("Banana"))
        {
            hasBanana = true;
            string message = "You got a Holy Banana!!!";
            other.gameObject.SetActive(false);
            showPopUp(message);
        }
        else if (other.gameObject.CompareTag("End"))
        {
            string message;
            if (hasBanana)
            {
                message = "Mission accomplished!";
                // TODO Level completed...
                LoadScene(nextLevel);
            }
            else
            {
                message = "You need to find a Holy Banana to complete the mission!!!";
            }
            showPopUp(message);
        }
    }

    void controlLight()
    {
        if (offTheBottom)
        {
            float currentLightRange = diversLight.GetComponent<Light>().range;
            diversLight.GetComponent<Light>().range = Mathf.Lerp(currentLightRange, defaultLightRange, 0.2f * Time.deltaTime);
        }

        newPosition = ridgidbody.transform.position;

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

        if (health > 50)
        {
            healthText.color = Color.green;
        }
        else
        {
            healthText.color = Color.red;
        }

        healthText.text = "Health: " + health.ToString();

        if (breathingGasAmount > 100)
        {
            breathingGasText.color = Color.green;
        }
        else
        {
            breathingGasText.color = Color.red;
        }
        breathingGasText.text = "Gas: " + Mathf.RoundToInt(breathingGasAmount).ToString();

        float buoyancy = calculateBuoyancy();
        buoyancyText.text = "B: " + buoyancy.ToString();
    }

    void showPopUp(string message)
    {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

                Handheld.Vibrate();
#endif

        popUp.text = message;
        Instantiate(popUp, transform.position, transform.rotation);
    }

    void manageBreathingGas()
    {
        breathingGasAmount -= Time.deltaTime;
    }

    public void LoadScene(string level)
    {
        SceneManager.LoadScene(level);
    }
}
