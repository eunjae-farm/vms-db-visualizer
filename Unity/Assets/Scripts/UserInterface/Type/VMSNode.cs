using System;
using Newtonsoft.Json;

public class VMSNode
{
    [JsonProperty("active")]
    public int Active { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("node")]
    public int NodeId { get; set; }
    [JsonProperty("nodetype")]
    public int NodeType { get; set; }
    [JsonProperty("parent")]
    public int Parent { get; set; }
    [JsonProperty("status")]
    public int Status { get; set; }
    [JsonProperty("treetype")]
    public int TreeType { get; set; }
}
