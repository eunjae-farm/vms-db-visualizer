using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupForAlarm : MonoBehaviour
{
    public bool AutoClose = false;
    public int AutoCloseTime = 3;

    public TMPro.TMP_Text Title;
    public UnityEngine.UI.Image ImageColor;

    public enum ButtonType
    {
        Warring,
        Error,
        Default
    }

    public void SetText(string text)
    {
        Title.text = text;
    }

    public void Open(ButtonType type, string text)
    {
        UnityThread.executeInUpdate(() =>
        {
            SetText(text);
            switch (type)
            {
                case ButtonType.Default:
                    this.ImageColor.color = new Color(127 / 255, 127 / 255, 127 / 255);
                    break;
                case ButtonType.Error:
                    this.ImageColor.color = new Color(255 / 255, 134 / 255, 134 / 255);
                    break;
                case ButtonType.Warring:
                    this.ImageColor.color = new Color(255 / 255, 248 / 255, 134 / 255);
                    break;
            }

            this.gameObject.SetActive(true);
            if (AutoClose)
            {
                StartCoroutine(AutoCloseFunc());
            }
        });
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public IEnumerator AutoCloseFunc()
    {
        yield return new WaitForSeconds(AutoCloseTime);
        Close();
    }

}
