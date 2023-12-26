using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DateForPicker : MonoBehaviour
{
    public DateTime SelectTime;
    
    [SerializeField] 
    public int Year;
    [SerializeField] 
    public int Month;

    public TMPro.TMP_Text Title;

    [SerializeField] private GameObject SixWeek;
    [SerializeField] GameObject DateForCalendar;

    public void PrevYear()
    {
        var y = new DateTime(Year, Month, 1).AddYears(-1);
        Year = y.Year;
        Month = y.Month;
        Load();
    }
    
    public void PrevMonth()
    {
        var y = new DateTime(Year, Month, 1).AddMonths(-1);
        Year = y.Year;
        Month = y.Month;
        Load();
    }
    
    public void NextYear()
    {
        var y = new DateTime(Year, Month, 1).AddYears(1);
        Year = y.Year;
        Month = y.Month;
        Load();
    }
    
    public void NextMonth()
    {
        var y = new DateTime(Year, Month, 1).AddMonths(1);
        Year = y.Year;
        Month = y.Month;
        Load();
    }
    
    List<List<Button>> buttons = new ();

    public void Awake()
    {
        int week = DateForCalendar.transform.childCount;
        for (int w = 0; w < week; w++)
        {
            var week_object = DateForCalendar.transform.GetChild(w);
            int day = week_object.childCount;
            List<Button> b = new();
            for (int d = 0; d < day; d++)
            {
                var button = week_object.GetChild(d).GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    if (button.tag == "O")
                    {
                        int day = int.Parse(button.GetComponentInChildren<TMP_Text>().text);
                    }
                    SelectTime = new(Year, Month, day);
                });
                b.Add(button);
            }
            buttons.Add(b);
        }
    }

    [EasyButtons.Button("Load")]
    public void Load()
    {
        SixWeek.SetActive(false);
        int countOfSat = 0;
        // Entry Start
        int count = 0;
        // 전날 부터 날짜 측정하여, 이전의 토요일을 찾을 때 까지 증감시킴

        for (var t = new DateTime(Year, Month, 1).AddDays(-1);
             t.DayOfWeek != DayOfWeek.Saturday;
             t = t.AddDays(-1), count += 1)
        {
            buttons[countOfSat][(int)t.DayOfWeek].GetComponentInChildren<TMP_Text>().text = $"{t.Day}";
            buttons[countOfSat][(int)t.DayOfWeek].tag = "X";

            buttons[countOfSat][(int)t.DayOfWeek].interactable = false;
        }

        for (int i = 0; countOfSat < 6; i++)
        {
            var t = new DateTime(Year, Month, 1).AddDays(i);
            if (countOfSat == 5 && t.Month == Month)
            {
                SixWeek.SetActive(true);
            }

            if (t.Month == Month)
            {
                buttons[countOfSat][(int)t.DayOfWeek].interactable = true;    
                buttons[countOfSat][(int)t.DayOfWeek].tag = "O";
            }
            else
            {
                buttons[countOfSat][(int)t.DayOfWeek].interactable = false;
                buttons[countOfSat][(int)t.DayOfWeek].tag = "X";
            }
            
            switch (t.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    buttons[countOfSat][(int)t.DayOfWeek].GetComponentInChildren<TMP_Text>().text = $"{t.Day}";
                    countOfSat += 1;
                    break;
                default:
                    buttons[countOfSat][(int)t.DayOfWeek].GetComponentInChildren<TMP_Text>().text = $"{t.Day}";
                    break;
            }
        }
    }
    
    // 정말 구현하기 귀찮아서 구현하는 방식입니다.
    // 실제로는 구현할 때, 수학적인 모델링을 통해 구해야 하지만,, 저는 뇌 빼고 개발하고 싶은걸요.
    // 그래서 모든 반복문을 돌아서 찾을겁니다 하하!
    
    
}
