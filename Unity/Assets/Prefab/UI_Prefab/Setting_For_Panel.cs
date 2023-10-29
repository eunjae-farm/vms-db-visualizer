using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_For_Panel : MonoBehaviour
{
    public TMPro.TMP_Text Date;
    public TMPro.TMP_Text NodeNmae;
    public TMPro.TMP_Text Title;
    public TMPro.TMP_Text Status;

    public void SetAlarm(VMSAlarmWithNode data)
    {
        this.Date.text = data.Date;
        this.NodeNmae.text = $"Node : {data.NodeName}";
        this.Title.text = $"Title : {data.Title}";
        this.Status.text = $"Status : {data.Status}";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
