using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCell_GameMenuController : MonoBehaviour
{
    public GameObject _inGameMenu;

    public void ToggleGameMenu()
    {
        _inGameMenu.SetActive(!_inGameMenu.activeSelf);
    }

}
