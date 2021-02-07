using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;

    private void Start()
    {
        startButton.onClick.AddListener(PlayGame);
        quitButton.onClick.AddListener(Quit);
    }
    private void PlayGame()
    {
        // TODO: Put the Level selection scene into build.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Quit()
    {
        Application.Quit();
    }
}
