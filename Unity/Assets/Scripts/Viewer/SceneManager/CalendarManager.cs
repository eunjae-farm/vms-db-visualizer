using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Dates;
using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    public GameObject Calendar;
    public DateForPicker picker;
    public MoreDetailTurbine MoreDetailTurbine;

    private void Awake()
    {
        picker.Clicked += PickerOnClicked; 
        picker.DateTimeChanged += PickerOnDateTimeChanged;
    }

    private void PickerOnDateTimeChanged(DateTime obj)
    {
        var nodes = MoreDetailTurbine.nodeData;
        var o = obj.AddMonths(1);
        var result = Server.Instance.AvailableMonthData(nodes.Select(item => item.Node.NodeId).ToList(),
            obj.Year,
            obj.Month,
            o.Year,
            o.Month);
        Debug.Log(result);
        
        
        foreach (var datas in result.MeasurementDate)
        {
            var dateTime = DateTime.Parse(datas);
            picker.SetButton(dateTime.Day, new Color(0.7f,0.7f, 1f));
        }
        
        foreach (var alarm in result.Alarm)
        {
            var dateTime = DateTime.Parse(alarm.Date);
            picker.SetButton(dateTime.Day, new Color(1f,0.7f, 0.7f));
        }
        
    }

    private void PickerOnClicked(DateTime obj)
    {
    }

    public void Open()
    {
        Calendar.SetActive(true);
        var now = DateTime.Now;
        picker.Set(now.Year, now.Month);
    }

    public void Close()
    {
        Calendar.SetActive(false);
    }

    public void Load()
    {
        
    }
}
