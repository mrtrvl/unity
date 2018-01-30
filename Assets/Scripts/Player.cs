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

    private float damage;
    private float damageFactor;
    private Rigidbody2D ridgidbody;
    private float depth;
    private GameObject diversLight;
    private bool offTheBottom = true;

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
        showText();
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        ridgidbody.velocity = new Vector2(horizontalMove * horizontalSpeed, verticalMove);

        depth = baseDepth - ridgidbody.position.y;

        if (offTheBottom)
        {
            diversLight.GetComponent<Light>().range = Mathf.Lerp(diversLight.GetComponent<Light>().range, defaultLightRange, 0.2f * Time.deltaTime);
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
