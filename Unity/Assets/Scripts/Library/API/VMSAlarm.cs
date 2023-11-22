using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VMSAlarm
{
    [JsonProperty("date")]
    [field: SerializeField]
    public string Date{ get; set; }

    [JsonProperty("eu")]
    [field: SerializeField]
    public double Eu { get; set; }

    [JsonProperty("id")]
    [field: SerializeField]
    public int Id { get; set; }

    [JsonProperty("node")]
    [field: SerializeField]
    public int Node { get; set; }

    [JsonProperty("source")]
    [field: SerializeField]
    public int Source { get; set; }

    [JsonProperty("status")]
    [field: SerializeField]
    public int Status { get; set; }

    [JsonProperty("title")]
    [field: SerializeField]
    public string Title { get; set; }

    [JsonProperty("value")]
    [field: SerializeField]
    public double Value { get; set; }
}
