using System;
using System.Collections;
using System.Collections.Generic;
using UI.Dates;
using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    public GameObject Calendar;
    public DatePicker picker;


    public void Open()
    {
        Calendar.SetActive(true);   
    }

    public void Close()
    {
        Calendar.SetActive(false);
    }

    public void Load()
    {
        
    }
}
