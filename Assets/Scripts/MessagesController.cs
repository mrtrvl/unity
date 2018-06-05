using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagesController : MonoBehaviour {

    public string message;
    public TextMesh popUp;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!GameState.gameState.MessageTriggerWasAlreadyShown(gameObject.name))
                ShowPopUp(message);
            gameObject.SetActive(false);
        }
    }

    private void ShowPopUp(string messageText)
    {
        popUp.text = messageText;
        Instantiate(popUp, transform.position, transform.rotation);
        GameState.gameState.AddMessageTrigger(gameObject.name);
    }
}
