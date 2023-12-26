using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VMSMonth
{
    [JsonProperty("alarm")]
    [field: SerializeField]
    public VMSMonthAlarm[] Alarm { get; set; }

    [JsonProperty("meas")]
    [field: SerializeField]
    public string[] MeasurementDate { get; set; }
}

[Serializable]
public class VMSMonthAlarm
{
    [JsonProperty("date")]
    [field: SerializeField]
    public string Date { get; set; }

    [JsonProperty("status")]
    [field: SerializeField]
    public int AlarmStatus { get; set; }
}


