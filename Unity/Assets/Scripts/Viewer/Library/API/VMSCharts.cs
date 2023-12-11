using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VMSCharts
{
    [JsonProperty("data")]
    [field: SerializeField]
    public double[] Data { get; set; }

    [JsonProperty("duration")]
    [field: SerializeField]
    public float Duration { get; set; }
}
