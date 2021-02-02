using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] RectTransform panel;
    [SerializeField] Button quitButton;
    [SerializeField] Button continueButton;

    [HideInInspector]
    public bool pauseMenuOn = false;

    private void Awake()
    {
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
    }

    private void TurnOffPauseMenu()
    {
        // resume the game
        Time.timeScale = 1f;

        // closes the pause menu
        panel.gameObject.SetActive(false);
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void Continue()
    {
        TurnOffPauseMenu();
    }

}
