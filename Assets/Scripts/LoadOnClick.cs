using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public void OptionsBackButtonClicked(string level)
    {
        var comp = GetComponent("SoundSlider");
        print(comp.transform);
        this.LoadScene(level);
    }
}
