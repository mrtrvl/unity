using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{
    public static GameController gameController;
    private const string playerDataFileName = "/playerData.dat";
    private const string playerSettingsPrefsFileName = "/playerSettings.dat";

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
    public void SavePausedLevelGameStatus(PausedGamedStatus pausedGameStatus)
    {
        var binaryFormatter = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + playerDataFileName);

        //var playerData = new PlayerData
        //{
        //    Health = pausedGameStatus.Health,
        //    BreathingGas = pausedGameStatus.BreathingGas,
        //    PositionX = pausedGameStatus.PositionX,
        //    PositionY = pausedGameStatus.PositionY,
        //    PositionZ = pausedGameStatus.PositionZ,
        //    ScaleX = pausedGameStatus.ScaleX,
        //    ScaleY = pausedGameStatus.ScaleY,
        //    ScaleZ = pausedGameStatus.ScaleZ
        //};

        binaryFormatter.Serialize(file, pausedGameStatus);
        file.Close();
    }

    /// <summary>
    /// Loads the paused game status. Loads gamestatus from the previously saved file.
    /// </summary>
    /// <returns>The paused level game status.</returns>
    public PausedGamedStatus LoadPausedLevelGameStatus()
    {
        if (File.Exists(Application.persistentDataPath + playerDataFileName))
        {
            var binaryFormatter = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + playerDataFileName, FileMode.Open);
            var playerData = (PausedGamedStatus)binaryFormatter.Deserialize(file);
            file.Close();

            return playerData;
        }
        return null;
    }
}
