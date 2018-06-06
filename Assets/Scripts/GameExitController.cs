using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExitController : MonoBehaviour {

    /// <summary>
    /// Quits game.
    /// </summary>
    public void GameExit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Reset game state, options, levels etc.
    /// </summary>
    public void GameReset()
    {
        GameController.gameController.DeleteLevelStatus();
        GameController.gameController.DeleteCurrentGameplayState();
        GameController.gameController.DeleteOptions();
    }
}
