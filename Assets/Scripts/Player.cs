using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using bananaDiver.gasImageController;
using bananaDiver.healthImageController;
using bananaDiver.optionsController;
using bananaDiver.vibrationController;
using bananaDiver.chestController;
using bananaDiver.JellyfishController;

public class Player : MonoBehaviour {

    public Text timeText;
    // public Text damageText;
    public Text depthText;
    public Text collectedItemsText;
    public Text healthText;
    public Text breathingGasText;
    public Text buoyancyText;

    private AudioManager audioManager;
    public int score;

    public float horizontalSpeed = 1f;
    public float baseDepth = 20f;
    public float defaultLightRange = 12f;
    public float minimumLightRange = 7f;
    public float buoyancyFactor = 0.01f;

    public GameObject pickUpsPanel;
    public GameObject bubbles;

    public bool hasKey = false;
    public bool hasMap = false;
    //public bool hasExplosive = false;
    public bool hasBanana = false;

    public TextMesh popUp;

    public string nextLevel;

    public int pickUpsCount;

    //private float damage = 0f;
    //private float damageFactor = 0.01f;
    private Rigidbody2D ridgidbody;

    private GameObject diversLight;
    private bool offTheBottom = true;

    private GameObject diverSprite;
    private GameObject death;
    private Animator diversAnimation;

    private float depth;
    private float airVolume = 0f;

    private int collectedItemsCount = 0;
    public int health = 100;

    private float horizontalMove;
    private float verticalMove;

    public float breathingGasAmount = 20;

    private Vector3 oldPosition;
    private Vector3 newPosition;

    private Vector3 oldLampDirection = new Vector3(90, 0, 0);
    private Quaternion rotation;

    private int healthIncreaseStep = 20;
    private int healthDecreaseStep = 5;
    private int breathingGasIncreaseStep = 200;

    private float timeToNextBreath = 0;
    private GameObject bubblesObject;

    private GameObject jelly;

    private const float breathInterval = 10.0f;

    private bool adjustedAirVolume = false;

    private bool vibration;

    private void Awake()
    {
        audioManager = AudioManager.audioManager;
    }

    void Start () 
	{
        Time.timeScale = 1;

        vibration = vibrationController.vibrationOn;

        ridgidbody = GetComponent<Rigidbody2D>();

        death = GameObject.Find("Death");

        death.SetActive(false);

        diversLight = GameObject.Find("DiveLamp");
        diversLight.GetComponent<Light>().range = defaultLightRange;

        diverSprite = GameObject.Find("sukelduja");
        diversAnimation = diverSprite.GetComponent<Animator>();

        oldPosition = ridgidbody.transform.position;

        ManageBubbles();

        showText();

        pickUpsCount = calculatePickUpCount();
    }

	void Update ()
	{
        GasIconController.amountOfGas = breathingGasAmount;
        HealthIconController.amountOfHealth = health;

        //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER

        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        horizontalMove = CrossPlatformInputManager.GetAxis("Horizontal");
        verticalMove = CrossPlatformInputManager.GetAxis("Vertical");
        //Debug.Log(horizontalMove);
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

        checkIfStillAlive();

        calulateScore();
    }

    void checkIfStillAlive ()
    {
        if (breathingGasAmount <= 0 || health <= 0)
        {
            audioManager.StopSound("Game Theme");
            dead();
        }
    }

    void dead ()
    {
        death.SetActive(true);
        Time.timeScale = 0;
    }

    void ManageBubbles()
    {
        if (Time.time >= timeToNextBreath)
        {
            if (bubblesObject != null)
            {
                Destroy(bubblesObject);
            }
            audioManager.PlaySound("BreathingWithBubbles");
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
        float buoyancy = volumeUnderPressure;

        return buoyancy;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jellyfish"))
        {
            jelly = collision.gameObject;
            if (jelly.GetComponent<JellyfishController>().isDangerous)
            {
                health -= healthDecreaseStep;
                audioManager.PlaySound("Scream");
                string message = "Health -" + healthDecreaseStep.ToString();
                showPopUp(message);

                jelly.GetComponent<JellyfishController>().cannotHurtForAWhile();
            }
            
        }
        if (collision.gameObject.name == "Ceiling" || collision.gameObject.name == "Bottom")
        {
            if (!adjustedAirVolume)
            {
                airVolume = (depth / 10 + 1) / 10;
                adjustedAirVolume = true;
            }

        }
    }

    void OnCollisionStay2D (Collision2D collision)
    {
        

        if(collision.gameObject.name == "Bottom" && diversLight.GetComponent<Light>().range > minimumLightRange)
        {
            diversLight.GetComponent<Light>().range -= 0.1f;
            offTheBottom = false;
        }
        
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        offTheBottom = true;
        adjustedAirVolume = false;
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin") || other.gameObject.CompareTag("Emerald") || other.gameObject.CompareTag("Diamond"))
        {
            audioManager.PlaySound("Other");
            collectedItemsCount += 1;
            string message = "You got something valuable!";
            other.gameObject.SetActive(false);
            showPopUp(message);
            ChestController.AddToItems(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Key"))
        {
            hasKey = true;
            audioManager.PlaySound("Key");
            string message = ("You got a key!");
            showPopUp(message);
            ChestController.AddToItems(other.gameObject);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Medkit"))
        {
            audioManager.PlaySound("Other");
            health += healthIncreaseStep;
            other.gameObject.SetActive(false);
            string message = "Health +" + healthIncreaseStep.ToString();
            showPopUp(message);
            HealthIconController.gotHealth = true;
        }
        else if (other.gameObject.CompareTag("Tank"))
        {
            audioManager.PlaySound("Gas");
            breathingGasAmount += breathingGasIncreaseStep;
            string message = "Breathing gas +" + breathingGasIncreaseStep.ToString();
            showPopUp(message);
            other.gameObject.SetActive(false);
            GasIconController.gotTank = true;
        }
        else if (other.gameObject.CompareTag("TNT"))
        {
            //hasExplosive = true;
            audioManager.PlaySound("Other");
            string message = "You got a TNT!";
            showPopUp(message);
            ChestController.AddToItems(other.gameObject);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Map"))
        {
            hasMap = true;
            audioManager.PlaySound("Map");
            string message = "You got a Map!";
            showPopUp(message);
            ChestController.AddToItems(other.gameObject);
            other.gameObject.SetActive(false);
            other.gameObject.transform.position = new Vector3(100, 100, 100);
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
        //damageText.text = "Damage: " + damage.ToString();
        depthText.text = "Depth: " + depth.ToString();
        collectedItemsText.text = collectedItemsCount.ToString();

        if (health > 50)
        {
            //healthText.color = Color.green;
        }
        else
        {
            //healthText.color = Color.red;
        }

        healthText.text = health.ToString();

        if (breathingGasAmount > 100)
        {
            //breathingGasText.color = Color.green;
        }
        else
        {
            //breathingGasText.color = Color.red;
        }
        breathingGasText.text = Mathf.RoundToInt(breathingGasAmount).ToString();

        float buoyancy = calculateBuoyancy();
        buoyancyText.text = (Mathf.Round(buoyancy * 100) / 100).ToString();
    }

    void showPopUp(string message)
    {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

                if (vibration)
                {
                    Handheld.Vibrate();
                }
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

    private void calulateScore ()
    {
        int coins = ChestController.itemCount("Coin");
        int diamonds = ChestController.itemCount("diamond");
        int percentOfPickedPickUps = (int)((coins + diamonds) * 100 / pickUpsCount);

        score = (int)(breathingGasAmount + health) + percentOfPickedPickUps;
    }

    private int calculatePickUpCount()
    {
        int coinsCount = GameObject.FindGameObjectsWithTag("Coin").Length;
        int diamondsCount = GameObject.FindGameObjectsWithTag("Diamond").Length;

        return coinsCount + diamondsCount;
    }
}
