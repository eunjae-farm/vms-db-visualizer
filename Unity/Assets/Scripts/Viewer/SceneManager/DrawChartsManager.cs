using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public TMPro.TMP_Text OverallOfHorizontal;
    public TMPro.TMP_Text OverallOfVertical;
    public TMPro.TMP_Text OverallOfAxial;
    
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

    public void BackToMain()
    {
        isOpenChart = false;
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
        if (bearingSequnence.Count != 15)
        {
            throw new NotImplementedException();
        }

        Nodes.Clear();
        for (int i = 0; i < 5; i++)
        {
            var H = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[i * 3 + 0]);
            var V = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[i * 3 + 1]);
            var A = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[i * 3 + 2]);
            Nodes.Add(new UnityList<NodeData> { list = new List<NodeData> { H, V, A } });
        }
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
        var name = new List<string>
        {
            "Main Bearing", 
            "GearBox",
            "Generator",
            "DE",
            "NDE",
        };
        var overall = new List<TMPro.TMP_Text>
        {
            OverallOfHorizontal,
            OverallOfVertical,
            OverallOfAxial,
        };
        foreach (var o in overall)
        {
            o.gameObject.SetActive(false);
        }

        Charts.GetChartComponent<Title>().text = name[MachineIndex];
        Charts.GetChartComponent<YAxis>().axisLabel.formatter = "{value:f3}";
        Charts.GetChartComponent<XAxis>().axisLabel.formatter = "";
        Charts.GetChartComponent<Tooltip>().numericFormatter = "F3";
        
        Charts.RemoveData();
        var axis = "HVA";
        for (int i = 0; i < nodes.list.Count; i++)
        {
            if (nodes.list[i] == null)
            {
                continue;
            }
            int axi = nodes.list[i].Axis;
            overall[axi].gameObject.SetActive(true);
            overall[axi].text = $"{axis[axi]} : {nodes.list[i].Search.Value / 1000.0:F1} mm/s";
            
            if (mode)
            {
                Charts.GetChartComponent<YAxis>().minMaxType = Axis.AxisMinMaxType.Default;
                Charts.GetChartComponent<XAxis>().minMaxType = Axis.AxisMinMaxType.Default;

                var fft = Charts.AddSerie<Line>($"FFT {axis[axi]}");            
                var fftData = nodes.list[i].FFT;
                for (int c = 0; c < fftData.Frequency.Length; c += 1)
                {
                    int p = (int)c;
                    fft.AddXYData(fftData.Frequency[p], fftData.Intensity[p]);
                }
            }
            else
            {
                Charts.GetChartComponent<YAxis>().minMaxType = Axis.AxisMinMaxType.Custom;
                Charts.GetChartComponent<XAxis>().minMaxType = Axis.AxisMinMaxType.Custom;

                var m = nodes.list.SelectMany(item => item.Chart.Data)
                    .Select(item => Math.Abs((item)))
                    .Max();
                var mt = nodes.list.Select(item => item.Chart.Duration)
                    .Select(item => Math.Abs((item)))
                    .Select(item => (int)(item * 1000) / 1000.0)
                    .Max();
                
                Charts.GetChartComponent<YAxis>().max = +(m*1.1);
                Charts.GetChartComponent<YAxis>().min = -(m*1.1);
                Charts.GetChartComponent<XAxis>().max = mt;
                
                var chart = Charts.AddSerie<Line>($"Time {axis[axi]}");
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
