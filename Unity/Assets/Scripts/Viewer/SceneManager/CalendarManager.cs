using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Dates;
using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    public GameObject Calendar;
    public DatePicker picker;
    public MoreDetailTurbine MoreDetailTurbine; 

    public void Open()
    {
        Calendar.SetActive(true);
        var nodes = MoreDetailTurbine.nodeData;
        var result = Server.Instance.AvailableMonthData(nodes.Select(item => item.Node.NodeId).ToList(),
                                            picker.VisibleDate.Date.Year,
                                            picker.VisibleDate.Date.Month);
        Debug.Log(result);
        
        
        

    }

    public void Close()
    {
        Calendar.SetActive(false);
    }

    public void Load()
    {
        
    }
}
