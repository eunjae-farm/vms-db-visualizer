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

    public void Setup(int id, string name, string status, string active, int idx)
    {
        Image.color = idx % 2 == 0 ? Color.white : Color.gray;
        Id.text = $"{id}";
        Name.text = name;
        Status.text = status;
        Active.text = active;
    }
}
