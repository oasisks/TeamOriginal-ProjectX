using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("Canvas Elements")]
    [SerializeField] RectTransform panel;
    [SerializeField] RectTransform levelFinishedPanel;
    [SerializeField] Button quitButton;
    [SerializeField] Button continueButton;

    public bool pauseMenuOn = false;

    private void Awake()
    {
        // ensure that it is disabled
        panel.gameObject.SetActive(false);
        levelFinishedPanel.gameObject.SetActive(false);

        // initalize button
        // TODO: Create a button (sometime later ig)
        // quitButton.onClick.AddListener(Quit);
        // continueButton.onClick.AddListener(Continue);
    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenuOn)
        {
            TurnOnPauseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseMenuOn)
        {
            TurnOffPauseMenu();
        }
    }

    private void TurnOnPauseMenu()
    {
        // freeze the game
        Time.timeScale = 0f;

        // bring the pause menu 
        panel.gameObject.SetActive(true);
        pauseMenuOn = true;
    }

    private void TurnOffPauseMenu()
    {
        // resume the game
        Time.timeScale = 1f;

        // closes the pause menu
        panel.gameObject.SetActive(false);
        pauseMenuOn = false;
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void Continue()
    {
        TurnOffPauseMenu();
    }

    public void enableFinishedPanel()
    {
        levelFinishedPanel.gameObject.SetActive(true);
    }

}
