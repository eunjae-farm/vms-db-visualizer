using System;
using Newtonsoft.Json;

public class VMSNodeData
{
    [JsonProperty("SpectraEUType")]
    public int SpectraEUType { get; set; }

    [JsonProperty("SpectraScaling")]
    public int SpectraScaling { get; set; }

    [JsonProperty("date")]
    public string Date { get; set; }

    [JsonProperty("end_freq")]
    public float EndFrequency { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("node")]
    public int Node { get; set; }

    [JsonProperty("sample_rate")]
    public float SampleRate { get; set; }

    [JsonProperty("speed")]
    public float Speed { get; set; }

    [JsonProperty("speed_begin")]
    public float SpeedBegin { get; set; }

    [JsonProperty("speed_end")]
    public float SpeedEnd { get; set; }

    [JsonProperty("speed_max")]
    public float SpeedMax { get; set; }

    [JsonProperty("speed_min")]
    public float SpeedMin { get; set; }

    [JsonProperty("start_freq")]
    public float StartFrequency { get; set; }

    [JsonProperty("time_signal_lines")]
    public int TimeSignalLines { get; set; }

    [JsonProperty("value")]
    public double Value { get; set; }

}
