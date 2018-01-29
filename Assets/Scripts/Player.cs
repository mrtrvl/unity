using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public Text timeText;
    public Text damageText;

    private float damage;
    private float damageFactor;
    private Light diversLight;
    private float lightRange;

	void Start () 
	{
        damage = 0f;
        damageFactor = 0.01f;
        showText();
        diversLight = GetComponent<Light>();
        //diversLight.transform.position = new Vector3(0,0,5);
    }

	void Update ()
	{
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
            diversLight.range -= 0.1f;
        }
    }

    void showText ()
    {
        timeText.text = "Time: " + Time.time.ToString();
        damageText.text = "Damage: " + damage.ToString();
    }
}
