using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{
    public static GameController gameController;
    private const string playerDataFileName = "/playerData.dat";
    private const string playerOptionsFileName = "/playerOptions.dat";

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
            file.Close();
        }

        return gameOptions;
    }

    /// <summary>
    /// Loads file from the system.
    /// </summary>
    /// <returns>Filestream</returns>
    /// <param name="fileName">File name.</param>
    private FileStream LoadFile(string fileName)
    {
        FileStream file = null;

        if (File.Exists((Application.persistentDataPath + fileName)))
            file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);

        return file;
    }
}
