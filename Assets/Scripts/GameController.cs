using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection.Emit;
using System.Collections.Generic;
using NUnit.Framework;

public class GameController : MonoBehaviour
{
    public static GameController gameController;
    private const string playerDataFileName = "/playerData.dat";
    private const string playerOptionsFileName = "/playerOptions.dat";
    private const string playerFinalScoreFileName = "/playerFinalScore.dat";

    public static float soundVolume = 1.0f;
    public static float musicVolume = 1.0f;

    /// <summary>
    /// Singleton instance handling.
    /// </summary>
    void Awake()
    {
        if (gameController == null)
        {
            DontDestroyOnLoad(gameObject);
            gameController = this;
        }
        else if (gameController != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Saves the paused game status. Writes paused game into file.
    /// </summary>
    /// <param name="pausedGameStatus">Paused game status.</param>
    public void SavePausedLevelGameStatus(PausedGameStatus pausedGameStatus)
    {
        var binaryFormatter = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + playerDataFileName);
        binaryFormatter.Serialize(file, pausedGameStatus);
        file.Close();
    }

    /// <summary>
    /// Loads the paused game status. Loads gamestatus from the previously saved file.
    /// </summary>
    /// <returns>The paused level game status.</returns>
    public PausedGameStatus LoadPausedLevelGameStatus()
    {
        var file = LoadFile(playerDataFileName);
        PausedGameStatus playerData = null;

        if (file != null)
        {
            var binaryFormatter = new BinaryFormatter();
            playerData = (PausedGameStatus)binaryFormatter.Deserialize(file);
            file.Close();
        }
        return playerData;
    }

    /// <summary>
    /// Saves the options.
    /// </summary>
    /// <param name="gameOptions">Game options.</param>
    public void SaveOptions(GameOptions gameOptions)
    {
        var binaryFormatter = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + playerOptionsFileName);
        binaryFormatter.Serialize(file, gameOptions);
        file.Close();
    }

    /// <summary>
    /// Loads the options.
    /// </summary>
    /// <returns>The options.</returns>
    public GameOptions LoadOptions()
    {
        var file = LoadFile(playerOptionsFileName);
        GameOptions gameOptions = null;

        if (file != null)
        {
            var binaryFormatter = new BinaryFormatter();
            gameOptions = (GameOptions)binaryFormatter.Deserialize(file);
            soundVolume = gameOptions.Sound;
            musicVolume = gameOptions.Music;
            file.Close();
        }

        return gameOptions;
    }

    public void SaveFinalScore(LevelResult levelScore)
    {
        var gameResults = LoadFinalScore();
        List<LevelResult> results = new List<LevelResult>();

        var binaryFormatter = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + playerFinalScoreFileName);
        if (gameResults != null)
        {
            gameResults.Add(levelScore);
            binaryFormatter.Serialize(file, gameResults);
        }
        else
        {
            results.Add(levelScore);
            binaryFormatter.Serialize(file, results);
        }

        file.Close();
    }

    public List<LevelResult> LoadFinalScore()
    {
        var file = LoadFile(playerFinalScoreFileName);
        List<LevelResult> levelResult = null;

        if (file != null)
        {
            var binaryFormatter = new BinaryFormatter();
            levelResult = (List<LevelResult>)binaryFormatter.Deserialize(file);
            file.Close();
        }

        return levelResult;
    }

    /// <summary>
    /// Loads file from the system.
    /// </summary>
    /// <returns>Filestream</returns>
    /// <param name="fileName">File name.</param>
    private FileStream LoadFile(string fileName)
    {
        FileStream file = null;

        if (File.Exists(Application.persistentDataPath + fileName))
            file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);

        return file;
    }

    public void DeleteCurrentGameplayState()
    {
        DeleteFile(playerDataFileName);
    }

    public void DeleteFileByFileName()
    {
        DeleteFile(playerOptionsFileName);
    }

    private void DeleteFile(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            File.Delete(Application.persistentDataPath + fileName);
        }
            
    }
}

public static class LevelTag
{
    public const string Options = "Options";
    public const string Training = "Training";
    public const string Main = "Main";
    public const string Pause = "Pause";
    public const string LevelOne = "Level_01";
    public const string LevelTwo = "Level_02";
    public const string Choose_Level = "Choose_Level";
}
