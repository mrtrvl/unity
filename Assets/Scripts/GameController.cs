using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{
    public static GameController gameController;
    public float health;
    public float breathingGas;
    private const string playerDataFileName = "/playerData.dat";
    private Player player;

    void Awake()
    {
        if (gameController == null)
        {
            DontDestroyOnLoad(gameObject);
            gameController = this;
        }
        else if (gameController != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = FindObjectOfType(typeof(Player));
    }

    private void Update()
    {
        Debug.Log(player.health);
    }

    public void Save()
    {
        var binaryFormatter = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + playerDataFileName);

        var playerData = new PlayerData();
        playerData.health = health;
        playerData.breathingGas = breathingGas;

        binaryFormatter.Serialize(file, playerData);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + playerDataFileName))
        {
            var binaryFormatter = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + playerDataFileName, FileMode.Open);
            var playerData = (PlayerData)binaryFormatter.Deserialize(file);
            file.Close();

            health = playerData.health;
            breathingGas = playerData.breathingGas;
        }
    }
}

[Serializable]
class PlayerData 
{
    public float health;
    public float breathingGas;
}
