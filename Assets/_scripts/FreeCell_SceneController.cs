using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class FreeCell_SceneController : MonoBehaviour
{
    public GameObject _loadingScreen;
    public Text _loadingText;

    public string _currentScene;

    private bool _currentlyLoading;
    private AsyncOperation _loadingOperation;

    void Start()
    {
        _currentScene = "Title";
        DontDestroyOnLoad(this);
        _currentlyLoading = false;
    }

    void Update()
    {
        //loading screen progress bar. Even though level should load instantly and not even display a loading screen, it's good practice
        if (_currentlyLoading == true)
        {
            if (_loadingOperation.isDone != true)
            {
                float _progress = Mathf.Clamp01(_loadingOperation.progress / 0.9f);
                _loadingText.text = (_progress * 100) + " %";
                Debug.Log("loading progress: " + _progress);
            }
            else //when finished, close up
            {
                _currentlyLoading = false;
                _loadingScreen.SetActive(false);
            }
        }
    }

    public void SwitchScenes(int _scene)
    {
        _loadingScreen.SetActive(true);
        _currentlyLoading = true;

        switch (_scene)
        {
            case 0: //TitleScreen
                Debug.Log("Loading Title Screen");
                _loadingOperation = SceneManager.LoadSceneAsync("TitleScreen");
                _currentScene = "Title";
                break;
            case 1: //Game Board
                Debug.Log("Dealing some cards");
                _loadingOperation = SceneManager.LoadSceneAsync("GameBoard");
                _currentScene = "GameBoard";
                break;
        }
    }
}
