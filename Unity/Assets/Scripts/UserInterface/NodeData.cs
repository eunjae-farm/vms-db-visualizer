using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData
{
    public NodeData(VMSNode node, List<VMSFFT> fft, List<VMSCharts> charts, VMSNodeData search)
    {
        this.Node = node;
        this.FFT = fft;
        this.Chart = charts;
        this.Search = search;
    }

    public VMSNode Node { get; set; }
    public List<VMSFFT> FFT { get; set; }
    public List<VMSCharts> Chart { get; set; }
    public VMSNodeData Search { get; set; }
}
