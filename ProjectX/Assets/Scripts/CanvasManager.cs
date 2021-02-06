using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [Header("Canvas Elements")]
    [SerializeField] RectTransform panel;
    [SerializeField] RectTransform levelFinishedPanel;
    [SerializeField] Button quitButton;
    [SerializeField] Button resumeButton;
    [SerializeField] Button loadNewGameButton;

    public bool pauseMenuOn = false;
    public bool levelFinishedPanelOn = false;

    private void Awake()
    {
        // ensure that it is disabled
        panel.gameObject.SetActive(false);
        levelFinishedPanel.gameObject.SetActive(false);

        // initalize button
        // TODO: Create a button (sometime later ig)
        quitButton.onClick.AddListener(Quit);
        resumeButton.onClick.AddListener(Resume);
        loadNewGameButton.onClick.AddListener(NewGame);
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

    public void Quit()
    {
        Application.Quit();
    }

    private void Resume()
    {
        TurnOffPauseMenu();
        Debug.Log("I resume");
    }

    public void enableFinishedPanel()
    {
        levelFinishedPanel.gameObject.SetActive(true);
        levelFinishedPanelOn = true;
    }

    public void NewGame()
    {
        Debug.Log("Restart Level");
        Time.timeScale = 1f;

        // this restarts the current level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
