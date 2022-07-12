using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [Header("---- PLAYER INPUT ----")]
    [SerializeField] PlayerInput playerInput;

    [Header("---- AUDIO DATA ----")]
    [SerializeField] AudioData pauseSFX;
    [SerializeField] AudioData unpauseSFX;

    [Header("---- CANVAS ----")]
    [SerializeField] Canvas hUDCanvas;
    [SerializeField] Canvas menusCanvas;

    [Header("---- PLAYER INPUT ----")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button mainMenuButton;

    int buttonPressedParameterID = Animator.StringToHash("Pressed");

    void OnEnable()
    {
        playerInput.onPause += Pause;
        playerInput.onUnPause += Unpause;

        ButtonPressedBehaviour.buttonFunctionTable.Add(resumeButton.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(optionsButton.gameObject.name, OnOptionsBottonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(mainMenuButton.gameObject.name, OnMainMenuBottonClick);
    }

    void OnDisable()
    {
        playerInput.onPause -= Pause;
        playerInput.onUnPause -= Unpause;

        ButtonPressedBehaviour.buttonFunctionTable.Clear();
    }

    void Pause()
    {
        hUDCanvas.enabled = false;
        menusCanvas.enabled = true;
        GameManager.GameState = GameState.Paused;
        TimeController.Instance.Pause();
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);
        AudioManager.Instance.PlaySFX(pauseSFX);
    }

    void Unpause()
    {
        resumeButton.Select();
        resumeButton.animator.SetTrigger(buttonPressedParameterID);
        AudioManager.Instance.PlaySFX(unpauseSFX);
    }

    void OnResumeButtonClick()
    {
        hUDCanvas.enabled = true;
        menusCanvas.enabled = false;
        GameManager.GameState = GameState.Playing;
        TimeController.Instance.Unpause();
        playerInput.EnableGameplayInput();
        playerInput.SwitchToFixedUpdateMode();
    }

    void OnOptionsBottonClick()
    {
        UIInput.Instance.SelectUI(optionsButton);
        playerInput.EnablePauseMenuInput();
    }

    void OnMainMenuBottonClick()
    {
        menusCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }

}
