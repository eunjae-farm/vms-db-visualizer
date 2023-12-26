using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementFromVibDbForHour : MonoBehaviour
{
    public TMP_Text Time;
    public TMP_Text Node;
    public TMP_Text Status;
    public TMP_Text Value;
    public Button Background;

    private DateTime time;

    public event Action<DateTime> Clicked;

    public void OnClicked()
    {
        Clicked?.Invoke(time);
    }
    
    public void SetData(DateTime time, string nodeId, string status, string value, Color color)
    {
        this.time = time;
        Time.text = time.ToString();
        Node.text = nodeId;
        Status.text = status;
        Value.text = value;
        var c = Background.colors;
        c.normalColor = color;
        Background.colors = c;
    }
}
