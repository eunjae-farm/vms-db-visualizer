using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Setting_For_Panel : MonoBehaviour
{
    public TMPro.TMP_Text Date;
    public TMPro.TMP_Text NodeNmae;
    public TMPro.TMP_Text Title;
    public TMPro.TMP_Text Status;
    public event Action<GameObject, VMSAlarmWithNode> MouseClick;
    private VMSAlarmWithNode data;
    
    public enum ColorsForStatus : int
    {
        LowWarning  = 64,
        HighWarning = 128,
        HighAlarm = 256,
        LowAlarm = 512,
    }

    private Color ConvertStatusToColor(ColorsForStatus status)
    {
        switch (status)
        {
            case ColorsForStatus.HighAlarm:
                return new Color(1, 0.3254717f, 0.3254717f, 0.6235294f);
            case ColorsForStatus.LowAlarm:
                return new Color(1, 0.8254717f, 0.8254717f, 0.6235294f);
            case ColorsForStatus.HighWarning:
                return new Color(1, 1, 0.6179246f, 0.6235294f);
            case ColorsForStatus.LowWarning:
                return new Color(1, 1, 0.1839623f, 0.6235294f);
            default:
                return new Color();
        }
    }
    
    public void SetAlarm(VMSAlarmWithNode data)
    {
        this.data = data;
        this.Date.text = DateTimeOffset.Parse(data.Date, CultureInfo.InvariantCulture).UtcDateTime.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
        this.NodeNmae.text = $"Node : {data.NodeName}";
        this.Title.text = $"Title : {data.Title}";
        this.Status.text = $"Status : {data.GetStatus()}";
        this.GetComponent<Image>().color = ConvertStatusToColor((ColorsForStatus)data.Status);
    }

    protected void OnMouseClickEvent()
    {
        Debug.Log("OnMouseClickEvent!");
        MouseClick?.Invoke(this.gameObject, data);
    }
}
