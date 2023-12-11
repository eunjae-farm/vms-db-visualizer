using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VMSFFT
{
    [JsonProperty("freq")]
    [field: SerializeField]
    public double[] Frequency;

    [JsonProperty("itensitiy")]
    [field: SerializeField]
    public double[] Intensity;
}
