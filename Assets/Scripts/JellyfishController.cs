using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishController : MonoBehaviour {

	private float movementDestinationX;
	private float movementDestinationY;
	private bool movementDirection;
	private float movementDistance;
    private Vector3 targetDirection;
    private Quaternion rotation;

    // constants
    //	private float const 


    // Use this for initialization
    void Start() 
	{
        SetFinalDestination();
    }
	
	// Update is called once per frame
	void Update() 
	{
        MoveJellyFish();
	}

    void MoveJellyFish()
    {
        if (Mathf.Abs(transform.position.x - movementDestinationX) <= 0.1 &&
            Mathf.Abs(transform.position.y - movementDestinationY) <= 0.1)
        {
            SetFinalDestination();
        }
        transform.position += targetDirection * 0.1f * Time.deltaTime;
        RotateJellyFish();
    }

    void SetFinalDestination()
    {
        movementDirection = GetMovementDirection();
        movementDistance = GetRandomNumber(0.5f, 3.0f);
        SetMovementDestination();
        targetDirection = (new Vector3(movementDestinationX, movementDestinationY, 0) - transform.position).normalized;
    }

	bool GetMovementDirection()
	{
		return Random.value <= 0.5 ? true : false;
	}

	void SetMovementDestination()
	{
		// true = right side
		if (movementDirection) 
		{
			movementDestinationX = transform.position.x + movementDistance;
			
		} 
		else 
		{
            movementDestinationX = transform.position.x - movementDistance;
		}
        movementDestinationY = GetValidMovementDestinationY();
        Debug.DrawLine(transform.position, 
        new Vector3(movementDestinationX, movementDestinationY, 0), Color.red, 10000.0f);
        //Debug.Log("X: " + movementDestinationX.ToString() + ", Y: " + movementDestinationY.ToString());
			
	}

	float GetValidMovementDestinationY()
	{
		float tempYValue = transform.position.y + GetRandomNumber(-2.0f, 2.0f);
        int a = 0;
        while (a <= 100)
		{
            if (CheckForObstaclesInMovementDirection(tempYValue)) {
                //Debug.Log(a);
                break;
            }
				
			tempYValue = transform.position.y + GetRandomNumber(-2.0f, 2.0f);
            a++;
		}
		return tempYValue;
	}


	bool CheckForObstaclesInMovementDirection(float y)
	{
        Vector2 direction = new Vector2(movementDestinationX, y) - new Vector2(transform.position.x, transform.position.y);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2, 1 << LayerMask.NameToLayer("Foreground"));

        return hit.collider == null ? true : false;
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        SetFinalDestination();
    }


    float GetRandomNumber(float min, float max)
	{
		return Random.Range(min, max);
	}

    void RotateJellyFish()
    {
        if (targetDirection != new Vector3(0, 0, 0))
        {
            rotation = Quaternion.LookRotation(targetDirection);
            rotation *= Quaternion.Euler(0, 90, 90);
            transform.rotation = rotation;
            Debug.Log("Rotation: " + targetDirection.ToString());
        }
    }
}
