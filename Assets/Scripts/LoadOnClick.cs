using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour {

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.audioManager;
    }

    public void LoadScene(string level)
    {
        if (audioManager == null)
            audioManager = AudioManager.audioManager;
        audioManager.PlaySound("Button Click");
        SceneManager.LoadScene(level);
    }
}
