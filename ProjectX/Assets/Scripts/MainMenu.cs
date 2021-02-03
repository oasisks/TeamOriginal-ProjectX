using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button playButton;

    private void Start()
    {
        // add a listener
        playButton.onClick.AddListener(playGame);
    }
    public void playGame()
    {
        // TODO: Put the Level selection scene into build.
        SceneManager.LoadScene("LvlSelect");
    }
}
