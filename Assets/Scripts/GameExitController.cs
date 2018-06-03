using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExitController : MonoBehaviour {

    public void GameExit()
    {
        GameController.gameController.DeleteLevelStatus();
        GameController.gameController.DeleteCurrentGameplayState();
        GameController.gameController.DeleteOptions();
    }
}
