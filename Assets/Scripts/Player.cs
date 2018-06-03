using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using bananaDiver.gasImageController;
using bananaDiver.healthImageController;
using bananaDiver.optionsController;
//using bananaDiver.vibrationController;
using bananaDiver.chestController;
using bananaDiver.JellyfishController;
using bananaDiver.buoyancyController;
using bananaDiver.scoreController;
using System.Runtime.CompilerServices;

public class Player : MonoBehaviour {

    public bool levelCompletedTest = false;

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
    //public bool hasMap = false;
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
    private GameObject win;
    private Animator diversAnimation;
    private GameState gameState;
    private bool endMessageShown = false;

    private float depth;
    private float airVolume = 0f;

    private int collectedItemsCount = 0;
    public int health = 100;

    private float horizontalMove;
    private float verticalMove;

    public float breathingGasAmount = 100;

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

    private Scene activeScene;

    //private bool vibration;

    private void Awake()
    {
        audioManager = AudioManager.audioManager;
        gameState = GameState.gameState;
    }

    void Start () 
	{
        Time.timeScale = 1;

        //vibration = vibrationController.vibrationOn;

        ridgidbody = GetComponent<Rigidbody2D>();

        activeScene = SceneManager.GetActiveScene();

        death = GameObject.Find("Death");
        win = GameObject.Find("Win");

        death.SetActive(false);
        win.SetActive(false);

        diversLight = GameObject.Find("DiveLamp");
        diversLight.GetComponent<Light>().range = defaultLightRange;

        diverSprite = GameObject.Find("sukelduja");
        diversAnimation = diverSprite.GetComponent<Animator>();

        oldPosition = ridgidbody.transform.position;

        ManageBubbles();

        ShowText();

        pickUpsCount = CalculatePickUpCount();
    }

	void Update ()
	{
        // For testing...
        if (levelCompletedTest)
            end ();


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

        float buoyancy = CalculateBuoyancy();

        bananaDiver.buoyancyController.BuoyancySliderScript.yPosition = buoyancy;

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

        ManageBreathingGas();

        FlipToMoveDirection();

        ShowText();

        ControlLight();

        ManageBubbles();

        manageAnimation();

        checkIfStillAlive();

        CalulateScore();
    }

    void checkIfStillAlive ()
    {
        if (breathingGasAmount <= 0 || health <= 0)
        {
            audioManager.StopSound(AudioFile.GameTheme);
            dead();
        }
    }

    void dead ()
    {
        death.SetActive(true);
        GameState.gameState.HandleWinDeathInformationDialog();
        Time.timeScale = 0;
    }

    void end ()
    {
        string infoMessage = string.Empty;
        if (hasBanana || activeScene.name == LevelTag.Training)
        {
            win.SetActive(true);
            GameState.gameState.HandleWinDeathInformationDialog();
            int finalScore = calculateFinalScore();
            // Save final score
            GameState.gameState.SaveGameResult(finalScore);
            ScoreController.score = finalScore;
            ScoreController.levelCompleted = true;
            Time.timeScale = 0;
            //infoMessage = "Mission accomplished!";
            //// TODO Level completed...
            //LoadScene(nextLevel);
        }
        else
        {
            if (!endMessageShown)
            {
                infoMessage = "You need to find Jack Sparrow's compass to complete the mission!!!";
                ShowPopUp(infoMessage);
                endMessageShown = true;
            }
        } 
    }

    void ManageBubbles()
    {
        if (Time.time >= timeToNextBreath)
        {
            if (bubblesObject != null)
            {
                Destroy(bubblesObject);
            }
            audioManager.PlaySound(AudioFile.BreathingWithBubbles);
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

    void FlipToMoveDirection()
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

    float CalculateBuoyancy()
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
                audioManager.PlaySound(AudioFile.Scream);
                string message = "Health -" + healthDecreaseStep.ToString();
                ShowPopUp(message);

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
        if (collision.gameObject.name == "Bottom" && diversLight.GetComponent<Light>().range > minimumLightRange)
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
        var objectTag = other.gameObject.tag;
        switch (objectTag)
        {
            case ItemTag.Coin:
            case ItemTag.Emerald:
            case ItemTag.Diamond:
                audioManager.PlaySound(AudioFile.Coin);
                collectedItemsCount += 1;
                other.gameObject.SetActive(false);
                ShowPopUp("You got something valuable!");
                gameState.AddAccesory(other.gameObject.transform.position.sqrMagnitude);
                ChestController.AddToItems(objectTag);
                break;
            case ItemTag.Key:
                hasKey = true;
                audioManager.PlaySound(AudioFile.Key);
                ShowPopUp("You got a key!");
                ChestController.AddToItems(objectTag);
                gameState.AddAccesory(other.gameObject.transform.position.sqrMagnitude);
                other.gameObject.SetActive(false);
                break;
            case ItemTag.Medkit:
                audioManager.PlaySound(AudioFile.Other);
                health += healthIncreaseStep;
                other.gameObject.SetActive(false);
                gameState.AddAccesory(other.gameObject.transform.position.sqrMagnitude);
                ShowPopUp(string.Format("Health {0}", healthIncreaseStep.ToString()));
                HealthIconController.gotHealth = true;
                break;
            case ItemTag.Tank:
                audioManager.PlaySound(AudioFile.Gas);
                breathingGasAmount += breathingGasIncreaseStep;
                ShowPopUp(string.Format("Breathing gas {0}", breathingGasIncreaseStep.ToString()));
                other.gameObject.SetActive(false);
                gameState.AddAccesory(other.gameObject.transform.position.sqrMagnitude);
                GasIconController.gotTank = true;
                break;
            case ItemTag.Tnt:
                audioManager.PlaySound(AudioFile.Other);
                ShowPopUp("You got a TNT!");
                ChestController.AddToItems(objectTag);
                gameState.AddAccesory(other.gameObject.transform.position.sqrMagnitude);
                other.gameObject.SetActive(false);
                break;
            case ItemTag.Map:
                //hasMap = true;
                audioManager.PlaySound(AudioFile.Map);
                ShowPopUp("You got a Map!");
                ChestController.AddToItems(objectTag);
                gameState.AddAccesory(other.gameObject.transform.position.sqrMagnitude);
                other.gameObject.transform.position = new Vector3(100, 100, 100);
                break;
            case ItemTag.Banana:
                hasBanana = true;
                other.gameObject.SetActive(false);
                ShowPopUp("You got a Jack Sparrows's compass!!!");
                break;
            case ItemTag.End:
                end();
                break;
            default:
                break;
        }
    }

    void ControlLight()
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

    void ShowText ()
    {
        timeText.text = "Time: " + Time.time.ToString();
        depthText.text = "Depth: " + depth.ToString();
        collectedItemsText.text = collectedItemsCount.ToString();

        healthText.text = health.ToString();

        breathingGasText.text = Mathf.RoundToInt(breathingGasAmount).ToString();

        float buoyancy = CalculateBuoyancy();
        buoyancyText.text = (Mathf.Round(buoyancy * 100) / 100).ToString();
    }

    void ShowPopUp(string message)
    {
        popUp.text = message;
        Instantiate(popUp, new Vector3(transform.position.x, transform.position.y, -2), transform.rotation);
    }

    void ManageBreathingGas()
    {
        breathingGasAmount -= Time.deltaTime;
    }

    public void LoadScene(string level)
    {
        SceneManager.LoadScene(level);
    }

    private void CalulateScore ()
    {
        int coins = ChestController.ItemCount(ItemTag.Coin);
        int diamonds = ChestController.ItemCount(ItemTag.Diamond);
        int percentOfPickedPickUps = (int)((coins + diamonds) * 100 / pickUpsCount);

        score = (int)(breathingGasAmount + health) + percentOfPickedPickUps;
    }

    private int calculateFinalScore ()
    {
        int maxScore = 300;
        int percentage = (int)100 * score / maxScore;

        return (int)percentage / 30;
    }

    private int CalculatePickUpCount()
    {
        int coinsCount = GameObject.FindGameObjectsWithTag(ItemTag.Coin).Length;
        int diamondsCount = GameObject.FindGameObjectsWithTag(ItemTag.Diamond).Length;

        return coinsCount + diamondsCount;
    }
}

public static class ItemTag
{
    public const string Coin = "Coin";
    public const string Player = "Player";
    public const string Emerald = "Emerald";
    public const string Diamond = "Diamond";
    public const string Medkit = "Medkit";
    public const string Tank = "Tank";
    public const string Gas = "Gas";
    public const string Map = "Map";
    public const string Tnt = "TNT";
    public const string Banana = "Banana";
    public const string Key = "Key";
    public const string Other = "Other";
    public const string End = "End";
    public const string Gate = "Gate";
}
