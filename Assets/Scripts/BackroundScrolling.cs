using UnityEngine;
using System.Collections;

public class BackroundScrolling : MonoBehaviour
{

    public float speedFactor;

    private Vector2 currentPosition;

	void Start ()
    {
        currentPosition = transform.position;
    }
	
	void Update ()
    {
        float moveHorizontal = Input.GetAxis("Horizontal") / speedFactor;
        float moveVertical = Input.GetAxis("Vertical") / speedFactor;

        transform.position = currentPosition + (Vector2.left * moveHorizontal) + (Vector2.down * moveVertical);
        currentPosition = transform.position;
    }
}
