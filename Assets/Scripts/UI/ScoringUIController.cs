using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringUIController : MonoBehaviour
{
    [Header("---- BACKGROUND ----")]
    [SerializeField] Image background;
    [SerializeField] Sprite[] backgroundImages;

    [Header("---- SCORING SCREEN ----")]
    [SerializeField] Canvas scoringScreenCanvas;
    [SerializeField] Text playerScoreText;
    [SerializeField] Button buttonMainMenu;
    [SerializeField] Transform highScoreLeaderBoardContainer;

    [Header("---- HIGH SCORE SCREEN ----")]
    [SerializeField] Canvas newHighScoreScreenCanvas;
    [SerializeField] Button buttonCancel;
    [SerializeField] Button buttonSubmit;
    [SerializeField] InputField playerNameInputField;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ShowRandomBackground();

        if(ScoreManager.Instance.HasNewHighScore)
        {
            ShowNewHighScoreScreen();
        }
        else
        {
            showScoringScreen();
        }

        ButtonPressedBehaviour.buttonFunctionTable.Add(buttonMainMenu.gameObject.name, OnButtonMainMenuClicked);
        ButtonPressedBehaviour.buttonFunctionTable.Add(buttonSubmit.gameObject.name, OnButtonSubmitClicked);
        ButtonPressedBehaviour.buttonFunctionTable.Add(buttonCancel.gameObject.name, HideNewHighScoreScreen);

        GameManager.GameState = GameState.Scoring;
    }

    void OnDisable()
    {
        ButtonPressedBehaviour.buttonFunctionTable.Clear();
    }

    void ShowRandomBackground()
    {
        background.sprite = backgroundImages[Random.Range(0, backgroundImages.Length)];
    }

    void ShowNewHighScoreScreen()
    {
        newHighScoreScreenCanvas.enabled = true;
        UIInput.Instance.SelectUI(buttonCancel);
    }

    
    void HideNewHighScoreScreen()
    {
        newHighScoreScreenCanvas.enabled = false;
        ScoreManager.Instance.SavePlayerScoreData();
        ShowRandomBackground();
        showScoringScreen();
    }

    void showScoringScreen()
    {
        scoringScreenCanvas.enabled = true;
        playerScoreText.text = ScoreManager.Instance.Score.ToString();
        UIInput.Instance.SelectUI(buttonMainMenu);
        UpdateHighScoreLeaderBoard();
    }

    void UpdateHighScoreLeaderBoard()
    {
        var playerScoreList = ScoreManager.Instance.LoadPlayerScoreData().list;

        for(int i=0; i<highScoreLeaderBoardContainer.childCount; i++)
        {
            var child = highScoreLeaderBoardContainer.GetChild(i);

            child.Find("Rank").GetComponent<Text>().text = (i+1).ToString();
            child.Find("Score").GetComponent<Text>().text = playerScoreList[i].score.ToString();
            child.Find("Rank").GetComponent<Text>().text = playerScoreList[i].playerName.ToString();
        }
    }

    void OnButtonMainMenuClicked()
    {
        scoringScreenCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }

    void OnButtonSubmitClicked()
    {
        if(!string.IsNullOrEmpty(playerNameInputField.text))
        {
            ScoreManager.Instance.SetPlayerName(playerNameInputField.text);
        }

        HideNewHighScoreScreen();
    }

}
