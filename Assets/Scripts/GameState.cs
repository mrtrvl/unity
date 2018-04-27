using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using bananaDiver.JellyfishController;

public class GameState : MonoBehaviour
{
    public static GameState gameState;
    private const string pause = "Pause";
    private const string levelOne = "Level_01";
    private const string mainMenu = "Main";
    private const string options = "Options";
    private string previousScene = string.Empty;
    private string currentScene = string.Empty;
    private AudioManager audioManager;

    /// <summary>
    /// Singleton instance handling.
    /// </summary>
    void Awake()
    {
        if (gameState == null)
        {
            DontDestroyOnLoad(gameObject);
            gameState = this;
        }
        else if (gameState != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        audioManager = AudioManager.audioManager;
    }

    public void LoadScene(string sceneName)
    {
        currentScene = sceneName;
        switch (sceneName)
        {
            case pause:
                BackButtonPressed(sceneName);
                break;
            case levelOne:
                break;
            default:
                print("Default");
                break;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    /// <summary>
    /// Event handler for scene loading. Used as delegate.
    /// </summary>
    /// <param name="scene">Scene object.</param>
    /// <param name="mode">Load scene object.</param>
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        print(string.Format("Scene loaded {0}", scene.name));
        if (audioManager == null)
            audioManager = AudioManager.audioManager;
        switch (scene.name)
        {
            case levelOne:
                if (previousScene == pause)
                    SetGameStateAfterPause();
                audioManager.PlaySoundLoop("Game Theme");
                previousScene = scene.name;
                if (previousScene != mainMenu)
                    audioManager.StopSound(mainMenu);
                break;
            case pause:
                audioManager.StopAllAudio();
                audioManager.PlaySoundLoop(mainMenu);
                previousScene = scene.name;
                break;
            case mainMenu:
                audioManager.PlaySoundLoop(mainMenu);
                previousScene = scene.name;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Sets the game state after Pause scene.
    /// </summary>
    private void SetGameStateAfterPause()
    {
        if (previousScene == pause)
        {
            var lastGameStatus = GameController.gameController.LoadPausedLevelGameStatus();
            var playerGameObject = GetGameObject("Diver");
            var diveLampGameObject = GetGameObject("DiveLamp");
            if (playerGameObject != null && lastGameStatus != null && diveLampGameObject != null)
            {
                playerGameObject.transform.position = new Vector3(lastGameStatus.PositionX, lastGameStatus.PositionY,
                                                                  lastGameStatus.PositionZ);
                var playerComponent = playerGameObject.GetComponent<Player>();
                playerComponent.health = lastGameStatus.Health;
                playerComponent.breathingGasAmount = lastGameStatus.BreathingGas;
                playerComponent.transform.localScale = new Vector3(lastGameStatus.ScaleX, lastGameStatus.ScaleY,
                                                                   lastGameStatus.ScaleZ);
                //var diveLampTemp = diveLampGameObject.transform.eulerAngles;
                //diveLampTemp.x = lastGameStatus.Accessories.DiveLamp.RotationX;
                //diveLampTemp.y = lastGameStatus.Accessories.DiveLamp.RotationY;
                //diveLampTemp.z = lastGameStatus.Accessories.DiveLamp.RotationZ;
                diveLampGameObject.transform.rotation = new Quaternion(lastGameStatus.Accessories.DiveLamp.RotationX,
                                                                       lastGameStatus.Accessories.DiveLamp.RotationY,
                                                                       lastGameStatus.Accessories.DiveLamp.RotationZ, 0.0f);
                //diveLampGameObject.transform.eulerAngles = new Vector3(lastGameStatus.Accessories.DiveLamp.RotationX,
                //lastGameStatus.Accessories.DiveLamp.RotationY,
                //lastGameStatus.Accessories.DiveLamp.RotationZ);
            }
        }
    }

    public void ContinueGameFromPause()
    {
        SceneManager.LoadScene(levelOne);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetGameStateDefaultProperties()
    {

    }

    /// <summary>
    /// Back button pressed. Saves current game state and loads Pause scene.
    /// </summary>
    /// <param name="sceneName">Scene name.</param>
    private void BackButtonPressed(string sceneName)
    {
        var playerGameObject = GetGameObject("Diver");
        var diveLampGameObject = GetGameObject("DiveLamp");
        //var jellyFishGameObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Jellyfish"));
        if (playerGameObject != null && diveLampGameObject != null)
        {
            var gameStatus = CreateCurrentGameStatus(playerGameObject, diveLampGameObject);
            GameController.gameController.SavePausedLevelGameStatus(gameStatus);
            SceneManager.LoadScene(sceneName);
        }
    }

    // Save objects
    private PausedGamedStatus CreateCurrentGameStatus(GameObject playerGameObject, GameObject diveLampObject)
    {
        var playerComponent = playerGameObject.GetComponent<Player>();
        var pausedGameStatus = new PausedGamedStatus
        {
            Health = playerComponent.health,
            BreathingGas = playerComponent.breathingGasAmount,
            PositionX = playerGameObject.transform.position.x,
            PositionY = playerGameObject.transform.position.y,
            PositionZ = 0.0f,
            ScaleX = playerGameObject.transform.localScale.x,
            ScaleY = playerGameObject.transform.localScale.y,
            ScaleZ = playerGameObject.transform.localScale.z
        };
        pausedGameStatus.Accessories = new Accessories()
        {
            DiveLamp = new DiveLamp()
            {
                RotationX = diveLampObject.transform.rotation.eulerAngles.x,
                RotationY = diveLampObject.transform.rotation.eulerAngles.y,
                RotationZ = diveLampObject.transform.rotation.eulerAngles.z,
            }
        };

        return pausedGameStatus;
    }

    /// <summary>
    /// Gets the game object.
    /// </summary>
    /// <returns>The game object.</returns>
    /// <param name="gameObjectName">Game object name.</param>
    private GameObject GetGameObject(string gameObjectName)
    {
        return GameObject.Find(gameObjectName);
    }
}

#region Serializable classes

[Serializable]
public class PausedGamedStatus
{
    public int Health { get; set; }
    public float BreathingGas { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float ScaleZ { get; set; }
    public Accessories Accessories { get; set; }
    public Hazards Hazards { get; set; }
}

[Serializable]
public class Accessories
{
    public DiveLamp DiveLamp { get; set; }
    public bool HasMap { get; set; }
}

[Serializable]
public class Hazards
{
    public List<GameObject> JellyFishGameObjects { get; set; }
}

[Serializable]
public class DiveLamp
{
    public float RotationX { get; set; }
    public float RotationY { get; set; }
    public float RotationZ { get; set; }
}

[Serializable]
public class GameSettings 
{
        
}

[Serializable]
public class LevelStatus
{
    
}

#endregion