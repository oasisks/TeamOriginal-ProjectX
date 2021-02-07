using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameFinished : MonoBehaviour
{
    [SerializeField] Button goBackToMainMenuButton;
    [SerializeField] Button quitButton;

    private void Start()
    {
        goBackToMainMenuButton.onClick.AddListener(GoToMainMenu);
        quitButton.onClick.AddListener(Quit);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void Quit()
    {
        Application.Quit();
    }
}
