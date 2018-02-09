using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishController : MonoBehaviour {

	private float movementDestinationX;
	private float movementDestinationY;
	private bool movementDirection;
	private float movementDistance;

	// constants
//	private float const 


	// Use this for initialization
	void Start() 
	{
		movementDirection = GetMovementDirection();
		movementDistance = GetRandomNumber(0.5f, 3.0f);
		SetMovementDestination();
	}
	
	// Update is called once per frame
	void Update() 
	{
		

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
        Debug.DrawLine(transform.position, new Vector3(movementDestinationX, movementDestinationY, 0), Color.red, 10000.0f);
        Debug.Log("X: " + movementDestinationX.ToString() + ", Y: " + movementDestinationY.ToString());
			
	}

	float GetValidMovementDestinationY()
	{
		float tempYValue = transform.position.y + GetRandomNumber(-2.0f, 2.0f);
		while (true) 
		{
			if (!CheckForObstaclesInMovementDirection(tempYValue))
				break;
			tempYValue = transform.position.y + GetRandomNumber(-2.0f, 2.0f);
		}
		return tempYValue;
	}


	bool CheckForObstaclesInMovementDirection(float y)
	{
        return Physics.Raycast(transform.position, new Vector3 (movementDestinationX, y, 100));
	}


	float GetRandomNumber(float min, float max)
	{
		return Random.Range(min, max);
	}


}
