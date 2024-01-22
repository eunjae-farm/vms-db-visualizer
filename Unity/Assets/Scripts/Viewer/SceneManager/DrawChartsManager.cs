using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using XCharts.Runtime;

public class DrawChartsManager : MonoBehaviour
{
    public GameObject LeftSideUI;
    public GameObject RightSideUI;

    public GameObject InfoUI;

    public LineChart Charts;
    public RandomGeneratorMotion WindTurbine;

    [SerializeField]
    private List<NodeData> nodeData;
    [SerializeField]
    private TurbineConnectionData turbineConnection;

    private bool _isOpenChart = false;
    private bool isOpenChart {
        get {

            return _isOpenChart;
        }
        set {
            InfoUI.SetActive(value);
            _isOpenChart = value;
        }
    }

    private int MachineIndex = 0;
    public void ChartButton()
    {
        isOpenChart = false;
        WindTurbine.GetComponent<RandomGeneratorMotion>().OnOutline(MachineIndex, false);
    }
    
    public List<UnityList<NodeData>> Nodes;
    public void ConvertData(List<NodeData> data, List<string> bearingSequnence)
    {
        var MB_A_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[0]);
        var MB_H_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[1]);
        var MB_V_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[2]);
        
        var GE_GS_A_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[3]);
        var GE_GS_H_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[4]);
        var GE_GS_V_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[5]);

        var GE_RS_A_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[6]);
        var GE_RS_H_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[7]);
        var GE_RS_V_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[8]);

        Nodes.Clear();
        Nodes.Add(new UnityList<NodeData> { list = new List<NodeData> { MB_A_VEL, MB_H_VEL, MB_V_VEL } });
        Nodes.Add(new UnityList<NodeData> { list = new List<NodeData> { GE_GS_A_VEL, GE_GS_H_VEL, GE_GS_V_VEL } });
        Nodes.Add(new UnityList<NodeData> { list = new List<NodeData> { GE_RS_A_VEL, GE_RS_H_VEL, GE_RS_V_VEL } });
    }
    
    public void Setup(List<NodeData> nodeData, TurbineConnectionData connection)
    {
        this.nodeData = nodeData;
        this.turbineConnection = connection;
        ConvertData(nodeData, connection.ObserveBearing);
    }

    private int tmp_click = -1;
    public void ShowEvent(int click, string name)
    {
        if (click == 0)
        {
            return;
        }
        var pos = Input.mousePosition;

        var width = pos.x / Screen.width;
        //var height = pos.y / Screen.height;

        isOpenChart = true;
        if (width <= 0.5)
        {
            InfoUI.transform.SetParent(RightSideUI.transform);
        }
        else
        {
            InfoUI.transform.SetParent(LeftSideUI.transform);
        }
        InfoUI.GetComponent<RectTransform>().offsetMin = new Vector2();
        InfoUI.GetComponent<RectTransform>().offsetMax = new Vector2();
        MachineIndex = click - 1;

        bool p = (tmp_click != click);
        WindTurbine.GetComponent<RandomGeneratorMotion>().OnOutline(MachineIndex, p);
        if (p)
        {
            DrawChart(true);
            tmp_click = click;
        }
        else
        {
            isOpenChart = false;
            tmp_click = -1;
        }
        
    }
    
    // mode "TRUE" is FFT
    // mode "FALSE" is Charts
    public void DrawChart(bool mode)
    {
        var nodes = Nodes[MachineIndex];
        // var name = new List<string>
        // {
        //     "Main Bearing", 
        //     "GearBox",
        //     "Generator"
        // };
        // Charts.GetComponent<Title>().text = name[MachineIndex];

        Charts.RemoveData();
        var axis = "HVA";
        
        for (int i = 0; i < nodes.list.Count; i++)
        {
            if (nodes.list[i] == null)
            {
                continue;
            }
            if (mode)
            {
                var fft = Charts.AddSerie<Line>($"FFT {axis[i]}");            
                var fftData = nodes.list[i].FFT;
                for (int c = 0; c < fftData.Frequency.Length; c += 1)
                {
                    int p = (int)c;
                    fft.AddXYData(fftData.Frequency[p], fftData.Intensity[p]);
                }
            }
            else
            {
                var chart = Charts.AddSerie<Line>($"Time {axis[i]}");
                var chartData = nodes.list[i].Chart;
                var time = Enumerable.Range(1, chartData.Data.Length)
                    .Select(i => (double)i / chartData.Data.Length * chartData.Duration)
                    .ToArray();
                for (float c = 0; c < chartData.Data.Length; c += (chartData.Data.Length / 1000f))
                {
                    int p = (int)c;
                    chart.AddXYData(time[p], chartData.Data[p]);
                }
            }
        }
    }

}
