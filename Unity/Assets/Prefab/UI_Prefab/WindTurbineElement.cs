using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbineElement : MonoBehaviour
{
    public TMPro.TMP_Text Id;
    public TMPro.TMP_Text Name;
    public TMPro.TMP_Text Status;
    public TMPro.TMP_Text Active;
    public UnityEngine.UI.Image Image;

    public event WindTurbineElementClickEvent Clicked;

    int nodeId = 0;
    public void Setup(int id, string name, string status, string active, int idx)
    {
        nodeId = id;
        Image.color = idx % 2 == 0 ? Color.white : new Color(230f / 255, 230f / 255, 230f / 255);
        Id.text = $"{id}";
        Name.text = name;
        Status.text = status;
        Active.text = active;
    }

    public void OnClick()
    {
        Clicked?.Invoke(nodeId, Name.text);
    }
}

public delegate void WindTurbineElementClickEvent(int nodeId, string name);