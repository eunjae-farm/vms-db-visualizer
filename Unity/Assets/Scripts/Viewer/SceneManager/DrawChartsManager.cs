using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCharts.Runtime;

public class DrawChartsManager : MonoBehaviour
{
    public GameObject LeftSideUI;
    public GameObject RightSideUI;

    public GameObject InfoUI;

    public LineChart Charts;

    [EditorTools.Button]
    public void ShowLeft()
    {
        InfoUI.SetActive(true);
        InfoUI.transform.SetParent(LeftSideUI.transform);
    }

    public void ShowRight()
    {
        InfoUI.SetActive(true);
        InfoUI.transform.SetParent(RightSideUI.transform);
    }

    public void Close()
    {
        InfoUI.SetActive(false);

    }


}
