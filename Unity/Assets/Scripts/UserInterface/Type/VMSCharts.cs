using System;
using Newtonsoft.Json;

public class VMSCharts
{
    [JsonProperty("data")]
    public double[] Data { get; set; }

    [JsonProperty("duration")]
    public float Duration { get; set; }
}
