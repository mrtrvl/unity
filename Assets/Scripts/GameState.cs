using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using bananaDiver.JellyfishController;
using System.Reflection;
using System.Linq;
using bananaDiver.optionsController;
using UnityEngine.UI;
using bananaDiver.vibrationController;
using System.Runtime.CompilerServices;
using bananaDiver.chestController;

public class GameState : MonoBehaviour
{
    public static GameState gameState;
    public static string previousScene = string.Empty;
    public static bool isGameplayPaused = false;
    private string currentScene = string.Empty;
    private AudioManager audioManager;
    private List<float> accessories = new List<float>();
    private List<string> hazards = new List<string>();
    private List<string> messageTriggers = new List<string>();

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
        currentScene = sceneName;
        switch (sceneName)
        {
            case LevelTag.Pause:
                BackButtonPressed(sceneName);
                break;
            case LevelTag.LevelOne:
                break;
            default:
                break;
        }
    }

    public void AddAccesory(float accessoryItemId)
    {
        accessories.Add(accessoryItemId);
    }

    public void AddHazard(string hazardTag)
    {
        hazards.Add(hazardTag);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public bool HazardWasDestroyed(string hazardTag)
    {
        return hazards.Contains(hazardTag);
    }

    public bool MessageTriggerWasAlreadyShown(string messageTriggerName)
    {
        return messageTriggers.Contains(messageTriggerName);
    }

    public void AddMessageTrigger(string messageTriggerName)
    {
        messageTriggers.Add(messageTriggerName);
    }

    /// <summary>
    /// Event handler for scene loading. Used as delegate.
    /// </summary>
    /// <param name="scene">Scene object.</param>
    /// <param name="mode">Load scene object.</param>
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        LoadOptionsForAudio();

        switch (scene.name)
        {
            case LevelTag.LevelOne:
                if (isGameplayPaused)
                    SetGameStateAfterPause();
                AudioManager.audioManager.PlaySoundLoop(AudioFile.GameTheme);
                if (previousScene != LevelTag.Main)
                    AudioManager.audioManager.StopSound(AudioFile.Main);
                isGameplayPaused = false;
                previousScene = scene.name;
                break;
            case LevelTag.Training:
                if (isGameplayPaused)
                    SetGameStateAfterPause();
                AudioManager.audioManager.PlaySoundLoop(AudioFile.GameTheme);
                if (previousScene != LevelTag.Main)
                    AudioManager.audioManager.StopSound(AudioFile.Main);
                isGameplayPaused = false;
                previousScene = scene.name;
                break;
            case LevelTag.Pause:
                if (!isGameplayPaused)
                {
                    AudioManager.audioManager.StopAllAudio();
                    AudioManager.audioManager.PlaySoundLoop(LevelTag.Main);   
                }
                AudioManager.audioManager.StopSound(AudioFile.BreathingWithBubbles);
                AudioManager.audioManager.ChangeCurrentlyPlayingSoundVolume(AudioFile.Main, null);
                isGameplayPaused = true;
                break;
            case LevelTag.Main:
                AudioManager.audioManager.ChangeCurrentlyPlayingSoundVolume(AudioFile.Main, null);
                AudioManager.audioManager.StopSound(AudioFile.BreathingWithBubbles);
                ResetGameplayStatus();
                AudioManager.audioManager.PlaySoundLoop(AudioFile.Main);
                isGameplayPaused = false;
                break;
            case LevelTag.Choose_Level:
                AudioManager.audioManager.ChangeCurrentlyPlayingSoundVolume(AudioFile.Main, null);
                break;
            case LevelTag.Options:
                break;
            default:
                break;
        }
    }

    private void LoadOptionsForAudio()
    {
        var options = GameController.gameController.LoadOptions();
        if (options != null)
        {
            GameController.soundVolume = options.Sound;
            GameController.musicVolume = options.Music;
        }
    }

    public void HandleWinDeathInformationDialog()
    {
        AudioManager.audioManager.StopAllAudio();
        AudioManager.audioManager.PlaySound(AudioFile.Main);
        ResetGameplayStatus();
    }

    public void ResetGameplayStatus()
    {
        if (accessories != null)
            accessories.Clear();
        if (ChestController.items != null)
            ChestController.items.Clear();
        if (hazards != null)
            hazards.Clear();
        if (messageTriggers != null)
            messageTriggers.Clear();
        GameController.gameController.DeleteCurrentGameplayState();
    }

    /// <summary>
    /// Sets the game state after Pause scene.
    /// </summary>
    private void SetGameStateAfterPause()
    {
        var lastGameStatus = GameController.gameController.LoadPausedLevelGameStatus();
        if (lastGameStatus == null)
            return;
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
                ItemTag.Tnt,
                ItemTag.Compass,
                ItemTag.Rock1,
                ItemTag.Rock2
            };

            var accessoryObjects = new List<GameObject>();
            foreach (var tagItem in tags)
            {
                accessoryObjects.AddRange(GameObject.FindGameObjectsWithTag(tagItem).ToList());
            }

            foreach (var accessoryItem in accessories)
            {
                foreach (var accessoryGameObject in accessoryObjects)
                {
                    if (Mathf.Approximately(accessoryGameObject.transform.position.sqrMagnitude, accessoryItem))
                    {
                        Destroy(accessoryGameObject);
                    }
                    if (hazards.Contains(accessoryGameObject.tag))
                    {
                        Destroy(accessoryGameObject);
                    }
                }
            }
        }
    }

    public void ContinueGameFromPause()
    {
        SceneManager.LoadScene(previousScene);
    }

    public void QuitGame()
    {
        Application.Quit();
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

    public void SaveGameResult(int score)
    {
        var result = new LevelResult()
        {
            LevelName = previousScene,
            LevelFinalScore = score,
            Completed = true
        };
        GameController.gameController.SaveFinalScore(result);
    }

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
    public bool VibrationOn { get; set; }
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
public class LevelResults
{
    public List<LevelResult> LevelResultsList { get; set; }
}

[Serializable]
public class LevelResult
{
    public string LevelName { get; set; }
    public int LevelFinalScore { get; set; }
    public bool Completed { get; set; }
}

#endregion