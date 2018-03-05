using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    public Texture2D MapImage;
    public GameObject player;

    private bool shown = false;
    private bool isAppearing = false;
    private float timer = 2f;
    private bool hasMap;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        hasMap = player.GetComponent<Player>().hasMap;

        if (isAppearing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isAppearing = false;
                shown = true;
            }
        }
	}

    void OnMouseDown()
    {
        if (!shown && hasMap)
        {
            isAppearing = true;
        }
    }

    void OnGUI()
    {
        if (isAppearing)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MapImage);
        }
    }
}
