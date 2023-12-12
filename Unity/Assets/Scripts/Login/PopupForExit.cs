using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupForExit : MonoBehaviour
{
    public GameObject Popup;

    public void Yes()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void No()
    {
        this.Popup.SetActive(false);
    }

    public void Show()
    {
        this.Popup.SetActive(true);
    }
}
