using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEditor;
using UnityEngine;
using XCharts.Runtime;

public class DrawChartsManager : MonoBehaviour
{
    public GameObject LeftSideUI;
    public GameObject RightSideUI;

    public GameObject InfoUI;

    public LineChart Charts;
    public GeneratorMotion WindTurbine;

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
            if (value)
            {
                ButtonViewChart.text = "차트 닫기";
            }
            else
            {
                ButtonViewChart.text = "차트 보기";
            }

            InfoUI.SetActive(value);
            _isOpenChart = value;
        }
    }

    public TMPro.TMP_Text ButtonViewChart;

    public void ChartButton()
    {
        isOpenChart = !isOpenChart;
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
        
        var nodes = WindTurbine.CurrentConvetedData();
        
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

        DrawChart(nodes[click - 1]);
    }

    public void DrawChart(UnityList<NodeData> nodes)
    {
        Charts.RemoveData();
        for (int i = 0; i < nodes.list.Count; i++)
        {
            var fft = Charts.AddSerie<Line>($"FFT {i:D2}");
            //var chart = Charts.AddSerie<Line>($"Chart {i:D2}");

            var fftData = nodes.list[i].FFT;

            //var chartData = nodes.list[i].Chart;
            //var chartTime = time.list[i];
            for (int c = 0; c < fftData.Frequency.Length; c++)
            {
                fft.AddXYData(fftData.Frequency[c], fftData.Intensity[c]);
            }
            //for (int c = 0; c < fftData.Frequency.Length; c++)
            //{
            //    var t = (c / chartData.Data.Length) * chartData.Duration;
            //    chart.AddXYData(t, chartData.Data[i]);
            //}
        }
    }

}
