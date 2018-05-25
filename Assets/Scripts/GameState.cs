using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using bananaDiver.JellyfishController;
using System.Reflection;
using System.Linq;

public class GameState : MonoBehaviour
{
    public static GameState gameState;
    private const string pause = "Pause";
    private const string levelOne = "Level_01";
    private const string levelTraining = "Training";
    private const string mainMenu = "Main";
    private const string options = "Options";
    private string previousScene = string.Empty;
    private string currentScene = string.Empty;
    private AudioManager audioManager;
    private List<float> accessories = new List<float>();

    /// <summary>
    /// Singleton instance handling.
    /// </summary>
    private void Awake()
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
        if (accessories != null)
            print("accessories exists");
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

    public void AddAccesory(float accessoryItemId)
    {
        print(string.Format("Added accessory: {0}", accessoryItemId));
        accessories.Add(accessoryItemId);
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
            case levelTraining:
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
            case options:
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
            var jellyFishGameObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Jellyfish")).ToArray();

            if (playerGameObject != null && lastGameStatus != null && diveLampGameObject != null)
            {
                playerGameObject.transform.position = new Vector3(lastGameStatus.PositionX, lastGameStatus.PositionY,
                                                                  lastGameStatus.PositionZ);
                var playerComponent = playerGameObject.GetComponent<Player>();
                playerComponent.health = lastGameStatus.Health;
                playerComponent.breathingGasAmount = lastGameStatus.BreathingGas;
                playerComponent.transform.localScale = new Vector3(lastGameStatus.ScaleX, lastGameStatus.ScaleY,
                                                                   lastGameStatus.ScaleZ);
                diveLampGameObject.transform.rotation = new Quaternion(lastGameStatus.Accessories.DiveLamp.RotationX,
                                                                       lastGameStatus.Accessories.DiveLamp.RotationY,
                                                                       lastGameStatus.Accessories.DiveLamp.RotationZ, 0.0f);

                var jellyFishObjects = lastGameStatus.Hazards.JellyFishObjects.ToArray();
                if (jellyFishGameObjects.Length == lastGameStatus.Hazards.JellyFishObjects.Count)
                    for (var i = 0; i < jellyFishObjects.Length; i++)
                        jellyFishGameObjects[i].transform.position = new Vector3(jellyFishObjects[i].PositionX, 
                                                                                 jellyFishObjects[i].PositionY,
                                                                                 jellyFishObjects[i].PositionZ);
            }

            if (accessories != null)
            {
                List<string> tags = new List<string>()
                {
                    ItemTag.Tank,
                    ItemTag.Map,
                    ItemTag.Coin,
                    ItemTag.Medkit,
                    ItemTag.Diamond,
                    ItemTag.Emerald,
                    ItemTag.Key,
                    ItemTag.Tnt
                };

                var accessoryObjects = new List<GameObject>();
                foreach (var tag in tags)
                {
                    accessoryObjects.AddRange(GameObject.FindGameObjectsWithTag(tag).ToList());   
                }

                foreach (var accessoryItem in accessories)
                {
                    foreach (var accessoryGameObject in accessoryObjects)
                    {
                        if (accessoryGameObject.transform.position.sqrMagnitude == accessoryItem)
                        {
                            Destroy(accessoryGameObject);
                        }
                    }
                }
            }
        }
    }

    public void ContinueGameFromPause()
    {
        //print(string.Format("Current: {0}, previous: {1}", currentScene, previousScene));
        SceneManager.LoadScene(currentScene);
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
        currentScene = SceneManager.GetActiveScene().name;
        var playerGameObject = GetGameObject("Diver");
        var diveLampGameObject = GetGameObject("DiveLamp");
        var jellyFishGameObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Jellyfish"));
        if (playerGameObject != null && diveLampGameObject != null)
        {
            var gameStatus = CreateCurrentGameStatus(playerGameObject, diveLampGameObject, currentScene, accessories, jellyFishGameObjects);
            GameController.gameController.SavePausedLevelGameStatus(gameStatus);
            SceneManager.LoadScene(sceneName);
        }
    }

    // Save objects
    private PausedGameStatus CreateCurrentGameStatus(GameObject playerGameObject, GameObject diveLampObject, 
                                                     string levelPausedFrom, List<float> collectibles, 
                                                     List<GameObject> jellyFishGameObjects)
    {
        var playerComponent = playerGameObject.GetComponent<Player>();
        var pausedGameStatus = new PausedGameStatus
        {
            LevelPausedFrom = levelPausedFrom,
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
            Collectibles = collectibles,
            DiveLamp = new DiveLamp()
            {
                RotationX = diveLampObject.transform.rotation.eulerAngles.x,
                RotationY = diveLampObject.transform.rotation.eulerAngles.y,
                RotationZ = diveLampObject.transform.rotation.eulerAngles.z,
            }
        };

        pausedGameStatus.Hazards = new Hazards()
        {
            JellyFishObjects = ConvertJellyFishGameObjects(jellyFishGameObjects)
        };

        return pausedGameStatus;
    }

    //private List<Collectible> GetCollectibles()
    //{
    //    var coins = GameObject.FindGameObjectsWithTag(ItemTag.Coin);
    //    var tanks = GameObject.FindGameObjectsWithTag(ItemTag.Tank);
    //    var emeralds = GameObject.FindGameObjectsWithTag(ItemTag.Emerald);
    //    var diamonds = GameObject.FindGameObjectsWithTag(ItemTag.Diamond);

    //}

    private List<JellyFishHazard> ConvertJellyFishGameObjects(List<GameObject> jellyFishGameObjects)
    {
        List<JellyFishHazard> serializableJellyFishObjects = new List<JellyFishHazard>();
        foreach (var go in jellyFishGameObjects)
        {
            var jellyFishComponent = go.GetComponent<JellyfishController>();
            if (jellyFishComponent != null)
            {
                var jellyFishHazard = new JellyFishHazard()
                {
                    PositionX = jellyFishComponent.transform.position.x,
                    PositionY = jellyFishComponent.transform.position.y,
                    PositionZ = 0.0f
                };
                serializableJellyFishObjects.Add(jellyFishHazard);
            }
        }

        return serializableJellyFishObjects;
    }

    private GameOptions CreateCurrentGameOptionsStatus()
    {
        
        return null;
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
public class PausedGameStatus
{
    public string LevelPausedFrom { get; set; }
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
    public List<float> Collectibles { get; set; }
    public bool HasMap { get; set; }
}

[Serializable]
public class Hazards
{
    public List<JellyFishHazard> JellyFishObjects { get; set; }
}

[Serializable]
public class DiveLamp
{
    public float RotationX { get; set; }
    public float RotationY { get; set; }
    public float RotationZ { get; set; }
}

[Serializable]
public class GameOptions 
{
    public float Sound { get; set; }
    public float Music { get; set; }
    public bool Vibration { get; set; }
}

[Serializable]
public class Collectible
{
    public string Name { get; set; }
    public bool? IsCollected { get; set; }
}

[Serializable]
public class JellyFishHazard 
{
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
}

[Serializable]
public class LevelStatus
{
    
}

#endregion