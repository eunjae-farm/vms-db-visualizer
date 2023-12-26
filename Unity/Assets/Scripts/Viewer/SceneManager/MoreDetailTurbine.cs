using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MoreDetailTurbine : SceneManager
{
    public GameObject UI;
    public GameObject SelectDateFor;
    
    public CameraSceneMove Cameras;
    public PopupForAlarm PopupAlarm;
    public GeneratorMotion TurbineMotion;
    public DrawChartsManager ChartManager;
    
    
    public void BackToMain()
    {
        Cameras.SetCamera(0);
    }

    public override void Enable()
    {
        UI.SetActive(true);
    }

    public override void Disable()
    {
        UI.SetActive(false);
    }
    
    public List<NodeData> nodeData { get; private set; }
    public TurbineConnectionData turbineConnection;
    public  List<VMSNode> vmsNode;
    public List<VMSAlarmWithNode> vmsAlarm;

    public void LoadForVibData(TurbineConnectionData data, List<VMSNode> node, List<VMSAlarmWithNode> alarm, DateTime start, DateTime end)
    {
        PopupAlarm.AutoClose = false;
        PopupAlarm.Open(PopupForAlarm.ButtonType.Warring, "데이터 로딩중");
        TurbineMotion.OutterBody(false);
        Task.Run(() =>
        {
            var s = new System.Diagnostics.Stopwatch();
            s.Start();

            var p = node.AsParallel()
                .Where(node => data.ObserveBearing.Select(item => item.ToLower())
                    .Contains(node.Name.ToLower()))
                        // .Where(node => !node.Name.ToLower().Contains("low"))
                        // .Where(node => !node.Name.ToLower().Contains("high"))
                        // .Where(node => !node.Name.ToLower().Contains("env"))
                        // .Where(node => !node.Name.ToLower().Contains("polar"))
                        // .Where(node => !node.Name.ToLower().Contains("hz"))
                        .Select(node => (start == DateTime.MinValue && end == DateTime.MinValue)
                                            ? (node, Server.Instance.Search(node.NodeId, 1, 0))
                                            : (node, Server.Instance.Search(node.NodeId, start, end)))
                        .Where(item => item.Item2 != null && item.Item2.Any())
                        .Select(item =>
                        {
                            return item;
                        })
                        .Select(item => (item.node, item.Item2.First()))
                        .Select(item => (item.node,
                                            search: item.Item2,
                                            fft: Server.Instance.fft(item.Item2.Id, item.Item2.TimeSignalLines, (int)item.Item2.EndFrequency),
                                            charts: Server.Instance.Charts(item.Item2.Id, (int)item.Item2.SampleRate)))
                        .Where(item => item.fft != null && item.charts != null)
                        .Select(item => new NodeData(item.node, item.fft, item.charts, item.search))
                        .OrderBy(item => item.Node.Name)
                        .ToList();

            turbineConnection = data;
            nodeData = p;
            vmsNode = node;
            vmsAlarm = alarm;

            s.Stop();
            Debug.Log(s.ElapsedMilliseconds);

            UnityThread.executeInUpdate(() =>
            {
                PopupAlarm.Close();
                TurbineMotion.OutterBody(true);
                TurbineMotion.SetData(nodeData, data.ObserveBearing);
                
                ChartManager.Setup(nodeData, turbineConnection);
            });
        });
    }

    private void Update()
    {
        //마우스 클릭시
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = Input.mousePosition;
            var cameras = Cameras.GetCameras();

            foreach (var camera in cameras.Cameras)
            {
                var rect = camera.pixelRect;
                if (rect.Contains(clickPosition))
                {
                    Ray ray = camera.ScreenPointToRay(clickPosition);
                    if (Physics.Raycast(ray, out var raycastHit, 1000f))
                    {
                        if (raycastHit.transform != null)
                        {
                            Transform t = raycastHit.transform;
                            if (t.gameObject.name.Contains("Terrain"))
                            {
                                return;
                            }

                            while (t.gameObject.name.Split("_")[0] != "00000") {
                                t = t.parent.transform;
                            }
                            int id = int.Parse(t.gameObject.name.Split("_")[1]);
                            ChartManager.ShowEvent(id, t.gameObject.name);
                            
                            Debug.Log(t.gameObject.name);
                        }
                    }
                }
            }
        }
    }
}
