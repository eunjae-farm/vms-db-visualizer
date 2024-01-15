using System.Collections;
using System.Collections.Generic;
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

    public void Setup(List<NodeData> nodeData, TurbineConnectionData connection)
    {
        this.nodeData = nodeData;
        this.turbineConnection = connection;
        
    }

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
        WindTurbine.GetComponent<RandomGeneratorMotion>().OnOutline(MachineIndex, true);

        DrawChart(true);
    }
    
    // mode "TRUE" is FFT
    // mode "FALSE" is Charts
    public void DrawChart(bool mode)
    {
        var nodes = WindTurbine.CurrentConvetedData()[MachineIndex];
        
        Charts.RemoveData();
        var axis = "XYZ";
        
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
                for (int c = 0; c < fftData.Frequency.Length; c++)
                {
                    fft.AddXYData(fftData.Frequency[c], fftData.Intensity[c]);
                }
            }
            else
            {
                var chart = Charts.AddSerie<Line>($"Chart {axis[i]}");
                var chartData = nodes.list[i].Chart;
                for (int c = 0; c < chartData.Data.Length; c++)
                {
                    var t = (c / chartData.Data.Length) * chartData.Duration;
                    chart.AddXYData(t, chartData.Data[i]);
                }
            }
        }
    }

}
