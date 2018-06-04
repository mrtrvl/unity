using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChooseLevelController : MonoBehaviour
{
    public Sprite levelTrainingZeroStars;
    public Sprite levelTrainingOneStar;
    public Sprite levelTrainingTwoStars;
    public Sprite levelTrainingThreeStars;
    public Sprite levelOneZeroStars;
    public Sprite levelTwoZeroStars;
    public Sprite levelLocked;

    private Button trainingButton;
    private Button level_02Button;
    private Button level_03Button;

    // Use this for initialization
    private void Start()
    {
        trainingButton = GameObject.Find("TrainingButton").GetComponent<Button>();
        level_02Button = GameObject.Find("Level_02Button").GetComponent<Button>();
        level_03Button = GameObject.Find("Level_03Button").GetComponent<Button>();

        Dictionary<string, Button> chooseLevelButtons = new Dictionary<string, Button>();
        chooseLevelButtons.Add(LevelTag.LevelOne, level_02Button);
        chooseLevelButtons.Add(LevelTag.LevelTwo, level_03Button);

        HandleLevelButtons(chooseLevelButtons);
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void HandleLevelButtons(Dictionary<string, Button> chooseLevelButtons)
    {
        var results = GameController.gameController.LoadFinalScore();
        Dictionary<string, LevelResult> levelScores = new Dictionary<string, LevelResult>();

        if (results != null)
        {
            foreach (var result in results.LevelResultsList)
            {
                LevelResult levelResultExists;
                levelScores.TryGetValue(result.LevelName, out levelResultExists);
                if (levelResultExists == null)
                    levelScores.Add(result.LevelName, result);
            }
        }

        LevelResult trainingLevelResult;
        levelScores.TryGetValue(LevelTag.Training, out trainingLevelResult);
        int trainingLevelScore = trainingLevelResult != null ? trainingLevelResult.LevelFinalScore : 0;
        SetSpriteImageForButton(trainingButton, trainingLevelScore);

        if (trainingLevelResult != null && trainingLevelResult.Completed)
        {
            level_02Button.interactable = true;
            level_02Button.image.sprite = levelOneZeroStars;
        }
        else
        {
            level_02Button.image.sprite = levelLocked;
            level_02Button.interactable = false;
        }

        LevelResult levelOneResult;
        levelScores.TryGetValue(LevelTag.LevelOne, out levelOneResult);

        if (levelOneResult != null && levelOneResult.Completed)
        {
            level_03Button.interactable = true;
            level_03Button.image.sprite = levelTwoZeroStars;
        }
        else
        {
            level_03Button.interactable = false;
            level_03Button.image.sprite = levelLocked;
        }
    }

    private void SetSpriteImageForButton(Button button, int score)
    {
        switch (score)
        {
            case 0:
                button.image.sprite = levelTrainingZeroStars;
                break;
            case 1:
                button.image.sprite = levelTrainingOneStar;
                break;
            case 2:
                button.image.sprite = levelTrainingTwoStars;
                break;
            case 3:
                button.image.sprite = levelTrainingThreeStars;
                break;
            default:
                break;
        }
    }
}

