using System;
using Newtonsoft.Json;

public class VMSAlarm
{
    [JsonProperty("date")]
    public string Date{ get; set; }

    [JsonProperty("eu")]
    public double Eu { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("duration")]
    public int Node { get; set; }

    [JsonProperty("source")]
    public int Source { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("value")]
    public double Value { get; set; }
}
