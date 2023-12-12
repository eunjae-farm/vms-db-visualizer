using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeData
{
    public NodeData(VMSNode node, VMSFFT fft, VMSCharts charts, VMSNodeData search)
    {
        this.Node = node;
        this.FFT = fft;
        this.Chart = charts;
        this.Search = search;
    }

    [field: SerializeField]
    public VMSNode Node { get; set; }
    [field: SerializeField]
    public VMSFFT FFT { get; set; }
    [field: SerializeField]
    public VMSCharts Chart { get; set; }
    [field: SerializeField]
    public VMSNodeData Search { get; set; }
}
