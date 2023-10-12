using System;
using Newtonsoft.Json;

public class VMSFFT
{
    [JsonProperty("freq")]
    public double[] Frequency;

    [JsonProperty("itensitiy")]
    public double[] Intensity;
}
