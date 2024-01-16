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
    
    [Header("Select Vibrarte")]
    public GameObject SelectVibDataInScrollView;
    public GameObject SelectVibDataInContentView;
    public GameObject SelectVibDataInPrefab;
    public List<GameObject> SelectVibData;

    public PopupForAlarm Alarm;
    public DateTime ClickedDateTime;
    
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
        ClickedDateTime = obj;
    }

    public void Open()
    {
        ClickedDateTime = DateTime.MinValue;
        Calendar.SetActive(true);
        var now = DateTime.Now;
        picker.Set(now.Year, now.Month);
    }

    public void Close()
    {
        Calendar.SetActive(false);
    }

    public void CloseSelectVibDataInScrollView()
    {
        SelectVibDataInScrollView.SetActive(false);
    }

    public void LoadFromHourPanel()
    {
        if (ClickDateTimeFromHours == DateTime.MinValue)
        {
            Alarm.Open(PopupForAlarm.ButtonType.Error, "관측하실 데이터를 선택하여 주시길 바랍니다.");
            return;
        }
        
        // MoreDetailTurbine
        MoreDetailTurbine.LoadForSelectTime(
            MoreDetailTurbine.turbineConnection, 
            MoreDetailTurbine.vmsNode,
            // MoreDetailTurbine.vmsAlarm,
            ClickedDateTime,
            60 /* 1 min */);
        
        Close();
        CloseSelectVibDataInScrollView();
    }

    private DateTime ClickDateTimeFromHours;
    private void OnClicked(DateTime obj)
    {
        ClickDateTimeFromHours = obj;
    }
    
    public void Load()
    {
        if (ClickedDateTime == DateTime.MinValue)
        {
            Alarm.Open(PopupForAlarm.ButtonType.Error, "날짜를 선택하여 주시길 바랍니다.");
            return;
        }
        ClickDateTimeFromHours = DateTime.MinValue;
        
        SelectVibDataInScrollView.SetActive(true);
        var nodes = MoreDetailTurbine.nodeData;
        var hour = Server.Instance.AvailableHourData(nodes.Select(item => item.Node.NodeId).ToList(),
            ClickedDateTime.Year, ClickedDateTime.Month, ClickedDateTime.Day);
        
        List<(DateTime, string, string, string, Color)> data = new ();
        foreach (var item in hour.Alarm)
        {
            data.Add(new ValueTuple<DateTime, string, string, string, Color>(
                DateTime.Parse(item.Date),
                nodes.Find(i => i.Node.NodeId == item.NodeId).Node.Name,
                "알림",
                VMSAlarm.GetStatus(item.Status),
                new Color(1f,0.7f, 0.7f)));
        }

        // 500개로 고정시키기 ->
        // 3000 -> 6
        // 2500 -> 5
        var step = (hour.MeasurementDate.Length / 500f);
        for (var i = 0f; i < hour.MeasurementDate.Length; i += step)
        {
            var idx = (int)i;
            var item = hour.MeasurementDate[idx];
            
            data.Add(new ValueTuple<DateTime, string, string, string, Color>(
                DateTime.Parse(item.MeasDate),
                nodes.Find(i => i.Node.NodeId == item.NodeId).Node.Name,
                "데이터",
                item.MeasValue.ToString("F3"),
                new Color(0.7f,0.7f, 1f)
            ));
        }
        
        
        int total_count = data.Count;
        
        for (int i = SelectVibData.Count; i < total_count; i++)
        {
            GameObject myInstance = Instantiate(SelectVibDataInPrefab, SelectVibDataInContentView.transform);
            myInstance.GetComponent<ElementFromVibDbForHour>().Clicked += OnClicked;
            SelectVibData.Add(myInstance);
        }

        data = data.OrderBy(d => d.Item1).ToList();
        
        for (int i = 0; i < total_count; i++)
        {
            SelectVibData[i].GetComponent<ElementFromVibDbForHour>().SetData(
                data[i].Item1, 
                data[i].Item2,
                data[i].Item3,
                data[i].Item4,
                data[i].Item5);
            
            SelectVibData[i].SetActive(true);
        }

        for (int i = total_count; i < SelectVibData.Count; i++)
        {
            SelectVibData[i].SetActive(false);
        }

    }

}
