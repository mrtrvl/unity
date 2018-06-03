using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using bananaDiver.optionsController;
using bananaDiver.vibrationController;

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

    public void TryAgainButtonClicked()
    {
        GameState.gameState.ResetGameplayStatus();
        SceneManager.LoadScene(GameState.previousScene);
    }

    public void OptionsBackButtonClicked()
    {
        var musicVolume = OptionsController.musicSliderValue;
        var soundVolume = OptionsController.soundSliderValue;
        var vibrationOn = vibrationController.vibrationOn;
        var gameOptions = new GameOptions()
        {
            Music = musicVolume,
            Sound = soundVolume,
            VibrationOn = vibrationOn
        };

        GameController.gameController.SaveOptions(gameOptions);

        if (GameState.isGameplayPaused)
            SceneManager.LoadScene(LevelTag.Pause);
        else
            SceneManager.LoadScene(LevelTag.Main);
    }

    public void HelpBackButtonClicked()
    {
        if (GameState.isGameplayPaused)
            SceneManager.LoadScene(LevelTag.Pause);
        else
            SceneManager.LoadScene(LevelTag.Main);
    }
}

