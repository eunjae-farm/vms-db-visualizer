using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Setting_For_Panel : MonoBehaviour
{
    public TMPro.TMP_Text Date;
    public TMPro.TMP_Text NodeNmae;
    public TMPro.TMP_Text Title;
    public TMPro.TMP_Text Status;
    private VMSAlarmWithNode data;
    public event Action<VMSAlarmWithNode> MouseClick;

    public void SetAlarm(VMSAlarmWithNode data)
    {
        this.data = data;
        this.Date.text = data.Date;
        this.NodeNmae.text = $"Node : {data.NodeName}";
        this.Title.text = $"Title : {data.Title}";
        this.Status.text = $"Status : {data.GetStatus()}";
    }

    public void OnMouseClickEvent()
    {
        Debug.Log("OnMouseClickEvent!");
        MouseClick?.Invoke(data);
    }
}
