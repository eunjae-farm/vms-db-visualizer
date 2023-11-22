using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VMSNodeData
{
    [JsonProperty("SpectraEUType")]
    [field: SerializeField]
    public int SpectraEUType { get; set; }

    [JsonProperty("SpectraScaling")]
    [field: SerializeField]
    public int SpectraScaling { get; set; }

    [JsonProperty("date")]
    [field: SerializeField]
    public string Date { get; set; }

    [JsonProperty("end_freq")]
    [field: SerializeField]
    public float EndFrequency { get; set; }

    [JsonProperty("id")]
    [field: SerializeField]
    public int Id { get; set; }

    [JsonProperty("node")]
    [field: SerializeField]
    public int Node { get; set; }

    [JsonProperty("sample_rate")]
    [field: SerializeField]
    public float SampleRate { get; set; }

    [JsonProperty("speed")]
    [field: SerializeField]
    public float Speed { get; set; }

    [JsonProperty("speed_begin")]
    [field: SerializeField]
    public float SpeedBegin { get; set; }

    [JsonProperty("speed_end")]
    [field: SerializeField]
    public float SpeedEnd { get; set; }

    [JsonProperty("speed_max")]
    [field: SerializeField]
    public float SpeedMax { get; set; }

    [JsonProperty("speed_min")]
    [field: SerializeField]
    public float SpeedMin { get; set; }

    [JsonProperty("start_freq")]
    [field: SerializeField]
    public float StartFrequency { get; set; }

    [JsonProperty("time_signal_lines")]
    public int TimeSignalLines { get; set; }

    [JsonProperty("value")]
    public double Value { get; set; }

}
