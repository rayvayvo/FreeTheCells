using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCell_TitleMenuController : MonoBehaviour
{
    public FreeCell_SceneController _sceneController;

    public void StartNewGame()
    {
        _sceneController.SwitchScenes(1);
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }
}
