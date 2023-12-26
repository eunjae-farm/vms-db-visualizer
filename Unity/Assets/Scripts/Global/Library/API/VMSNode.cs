using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VMSNode
{
    [JsonProperty("active")]
    [field: SerializeField]
    public int Active { get; set; }

    [JsonProperty("name")]
    [field: SerializeField]
    public string Name { get; set; }

    [JsonProperty("node")]
    [field: SerializeField]
    public int NodeId { get; set; }


    [JsonProperty("nodetype")]
    [field: SerializeField]
    public int NodeType { get; set; }

    [field: SerializeField]
    [JsonProperty("parent")]
    public int Parent { get; set; }

    [field: SerializeField]
    [JsonProperty("status")]
    public int Status { get; set; }

    [field: SerializeField]
    [JsonProperty("treetype")]
    public int TreeType { get; set; }
}
