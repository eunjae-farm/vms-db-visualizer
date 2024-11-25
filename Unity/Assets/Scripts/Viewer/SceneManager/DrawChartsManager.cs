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

    public GameObject SomeOfOverlay;
    public GameObject OneOfOverlay;
    
    public TMPro.TMP_Text OneOfOverallOfHorizontal;
    public TMPro.TMP_Text OneOfOverallOfVertical;
    public TMPro.TMP_Text OneOfOverallOfAxial;
    
    public TMPro.TMP_Text SomeOfOverallOfHorizontal;
    public TMPro.TMP_Text SomeOfOverallOfVertical;
    public TMPro.TMP_Text SomeOfOverallOfAxial;
    public TMPro.TMP_Text OtherOfOverallOfHorizontal;
    public TMPro.TMP_Text OtherOfOverallOfVertical;
    public TMPro.TMP_Text OtherOfOverallOfAxial;
    
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
        if (MachineIndex > 0)
        {
            WindTurbine.GetComponent<RandomGeneratorMotion>().OnOutline(MachineIndex, false);
        }
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
        
        var name = new List<string>
        {
            "Main Bearing", 
            "GearBox",
            "Generator",
        };
        var vvv = new List<List<(int, string)>>
        {
            new List<(int, string)>
            {
                (0, "RS"),
            },
            new List<(int, string)>
            {
                (1, "RS"),
                (2, "GS"),
            },
            new List<(int, string)>
            {
                (3, "DE"),
                (4, "NDE"),
            }
        };
        
        var oneofoverall = new List<TMPro.TMP_Text>
        {
            OneOfOverallOfHorizontal,
            OneOfOverallOfVertical,
            OneOfOverallOfAxial,
        };
        var someofoverall = new List<TMPro.TMP_Text>
        {
            SomeOfOverallOfHorizontal,
            SomeOfOverallOfVertical,
            SomeOfOverallOfAxial,
            OtherOfOverallOfHorizontal,
            OtherOfOverallOfVertical,
            OtherOfOverallOfAxial,
        };
            
        foreach (var o in oneofoverall)
        {
            o.gameObject.SetActive(false);
        }
        foreach (var o in someofoverall)
        {
            o.gameObject.SetActive(false);
        }

        Charts.GetChartComponent<Title>().text = name[MachineIndex];
        Charts.GetChartComponent<YAxis>().axisLabel.formatter = "{value:f3}";
        Charts.GetChartComponent<XAxis>().axisLabel.formatter = "";
        Charts.GetChartComponent<Tooltip>().numericFormatter = "F3";
        
        Charts.RemoveData();
        var axis = "HVA";
        Color[] colors = new[]
        {
            new Color(89f / 255, 112f / 255, 192.0f / 255),
            new Color(158f / 255,202f / 255,126f / 255),
            new Color(242f / 255,201f / 255,107f / 255),
        };
        Charts.GetChartComponent<YAxis>().max = 0;
        Charts.GetChartComponent<YAxis>().min = 0;
        Charts.GetChartComponent<XAxis>().max = 0;

        if (vvv[MachineIndex].Count == 1)
        {
            OneOfOverlay.SetActive(true);
            SomeOfOverlay.SetActive(false);
        }
        else
        {
            OneOfOverlay.SetActive(false);
            SomeOfOverlay.SetActive(true);
        }

        int upM = 0;
        foreach (var (midx, mname) in vvv[MachineIndex])
        {
            var nodes = Nodes[midx];
            
            for (int i = 0; i < nodes.list.Count; i++)
            {
                if (nodes.list[i] == null)
                {
                    continue;
                }

                int axi = i;
                // overall[addedNum].color = colors[addedNum];

                if (mode)
                {
                    Charts.GetChartComponent<YAxis>().minMaxType = Axis.AxisMinMaxType.Default;
                    Charts.GetChartComponent<XAxis>().minMaxType = Axis.AxisMinMaxType.Default;

                    if (nodes.list[i].FFT == null ||
                        nodes.list[i].FFT.Frequency == null ||
                        nodes.list[i].FFT.Intensity == null)
                    {
                        continue;
                    }

                    var fft = Charts.AddSerie<Line>($"FFT({mname}) {axis[axi]}");
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
                    if (nodes.list[i].Chart == null ||
                        nodes.list[i].Chart.Data == null)
                    {
                        continue;
                    }

                    var m = nodes.list
                        .Where(item => item != null)
                        .Where(item => item.Chart != null)
                        .Where(item => item.Chart.Data != null)
                        .SelectMany(item => item.Chart.Data)
                        .Select(item => Math.Abs((item)))
                        .Max();
                    var mt = nodes.list
                        .Where(item => item != null)
                        .Where(item => item.Chart != null)
                        .Select(item => item.Chart.Duration)
                        .Select(item => Math.Abs((item)))
                        .Select(item => (int)(item * 1000) / 1000.0)
                        .Max();

                    Charts.GetChartComponent<YAxis>().max += m * 1.1 / vvv[MachineIndex].Count;
                    Charts.GetChartComponent<YAxis>().min -= m * 1.1 / vvv[MachineIndex].Count;
                    Charts.GetChartComponent<XAxis>().max = Math.Max(mt, Charts.GetChartComponent<XAxis>().max);

                    var chart = Charts.AddSerie<Line>($"Time({mname}) {axis[axi]}");
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
                
                if (vvv[MachineIndex].Count == 1)
                {
                    OneOfOverlay.SetActive(true);
                    SomeOfOverlay.SetActive(false);
                }
                else
                {
                    OneOfOverlay.SetActive(false);
                    SomeOfOverlay.SetActive(true);
                }
                
                if (vvv[MachineIndex].Count == 1)
                {
                    oneofoverall[axi].gameObject.SetActive(true);
                    oneofoverall[axi].text = $"{axis[axi]} : {nodes.list[i].Search.Value:F1} mm/s";
                }
                else
                {
                    someofoverall[axi + upM * 3].gameObject.SetActive(true);
                    someofoverall[axi + upM * 3].text = $"({mname}){axis[axi]} : {nodes.list[i].Search.Value:F1}1";
                }
                // overall[axi].gameObject.SetActive(true);
                // overall[axi].text = $"{axis[axi]} : {nodes.list[i].Search.Value:F1} mm/s";
            }

            upM += 1;
        }

    }

}
