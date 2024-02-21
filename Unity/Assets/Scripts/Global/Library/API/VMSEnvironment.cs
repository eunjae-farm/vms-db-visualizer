using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VMSEnvironment
{
    [JsonProperty("temp")]
    [field: SerializeField]
    public double[] Temp { get; set; }

    [JsonProperty("sound")]
    [field: SerializeField]
    public float Sound { get; set; }
    
    [JsonProperty("dust")]
    [field: SerializeField]
    public float Dust { get; set; }
}