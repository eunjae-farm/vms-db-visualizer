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

    List<NodeData> nodeData;
    TurbineConnectionData turbineConnection;

    public void Setup(List<NodeData> nodeData, TurbineConnectionData connection)
    {
        this.nodeData = nodeData;
        this.turbineConnection = connection;
    }

    [Button]
    public void ShowLeft()
    {
        InfoUI.SetActive(true);
        InfoUI.transform.SetParent(LeftSideUI.transform);
        InfoUI.GetComponent<RectTransform>().offsetMin = new Vector2();
        InfoUI.GetComponent<RectTransform>().offsetMax = new Vector2();
    }

    [Button]
    public void ShowRight()
    {
        InfoUI.SetActive(true);
        InfoUI.transform.SetParent(RightSideUI.transform);
        InfoUI.GetComponent<RectTransform>().offsetMin = new Vector2();
        InfoUI.GetComponent<RectTransform>().offsetMax= new Vector2();
    }

    [Button]
    public void Close()
    {
        InfoUI.SetActive(false);
    }

}
