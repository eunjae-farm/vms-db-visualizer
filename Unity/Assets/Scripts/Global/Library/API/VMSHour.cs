using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VMSHour
{
    [JsonProperty("alarm")]
    [field: SerializeField]
    public VMSHourAlarm[] Alarm { get; set; }

    [JsonProperty("meas")]
    [field: SerializeField]
    public VMSHourMeas[] MeasurementDate { get; set; }
}

[Serializable]
public class VMSHourAlarm
{
    [JsonProperty("node_id")]
    [field: SerializeField]
    public int NodeId { get; set; }

    [JsonProperty("date")]
    [field: SerializeField]
    public string Date { get; set; }
    
    [JsonProperty("status")]
    [field: SerializeField]
    public int Status { get; set; }
}

[Serializable]
public class VMSHourMeas
{
    [JsonProperty("node_id")]
    [field: SerializeField]
    public int NodeId { get; set; }

    [JsonProperty("meas_value")]
    [field: SerializeField]
    public float MeasValue { get; set; }
    
    [JsonProperty("meas_date")]
    [field: SerializeField]
    public string MeasDate { get; set; }

} 