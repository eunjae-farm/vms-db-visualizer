using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MoreDetailTurbine : SceneManager
{
    public GameObject UI;
    
    public CameraSceneMove Cameras;
    public PopupForAlarm PopupAlarm;
    public GeneratorMotion TurbineMotion;
    public DrawChartsManager ChartManager;

    public TMP_Text InformationDateTime;
    public TMP_Text InformationDeviceStatus;
    public TMP_Text InformationProblem;
    
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

            var time = p.Select(item => item.Search.Date)
                .Select(item => DateTime.Parse(item))
                .OrderBy(item => item)
                .ToList();
            
            var nodeIds = node.AsParallel()
                .Where(node => data.ObserveBearing.Select(item => item.ToLower())
                    .Contains(node.Name.ToLower()))
                .Select(item => item.NodeId)
                .ToList();

            if (time.Count == 0)
            {
                UnityThread.executeInUpdate(() => 
                {
                    InformationDeviceStatus.text = "장비 상태 : 데이터 없음";    
                });
            }
            else
            {
                var tt = time[time.Count / 2];
                var stt = tt.AddHours(-1);
                var ett = tt.AddHours(+1);
                
                var alarms = Server.Instance.Alarm(start, end, nodeIds);
                string device_name = "";
                string status = "";
                int stat = 0;
                
                for (int i = 0; i < alarms.Count; i++)
                {
                    var date = DateTime.Parse(alarms[i].Date);

                    if (stt < date && date < ett)
                    {
                        if (alarms[i].Status > stat)
                        {
                            status = alarms[i].GetStatus();
                            device_name = alarm[i].NodeName;
                        }                        
                    }
                }

                UnityThread.executeInUpdate(() =>
                {
                    if (stat != 0)
                    {
                        InformationDeviceStatus.text = $"장비 상태 : {device_name} : ({status})";    
                    }
                    else
                    {
                        InformationDeviceStatus.text = $"장비 상태 : 알림 없음";
                    }
                    
                });
            }
            
            
            
            turbineConnection = data;
            nodeData = p;
            vmsNode = node;
            vmsAlarm = alarm;

            s.Stop();
            Debug.Log(s.ElapsedMilliseconds);

            UnityThread.executeInUpdate(() =>
            {
                if (time.Count == 0)
                {
                    InformationDateTime.text = "날짜 : " + "데이터를 찾을 수 없습니다.";                    
                }
                else
                {
                    InformationDateTime.text = "날짜 : " + time[time.Count / 2].ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
                }
                
                PopupAlarm.Close();
                TurbineMotion.OutterBody(true);
                TurbineMotion.SetData(nodeData, data.ObserveBearing);
                
                ChartManager.Setup(nodeData, turbineConnection);

                var nodes = TurbineMotion.Nodes;
                var axis = "XYZ";
                var dev = new List<string>
                {
                    "MB",
                    "RS",
                    "GS"
                };
                
                var error = new List<string>();
                for (int g = 0; g < nodes.Count; g++)
                {
                    string tmp = $"{dev[g]} (";
                    bool isAdd = false;
                    for (int d = 0; d < nodes[g].list.Count; d++)
                    {
                        List<string> ax = new();
                        
                        if (nodes[g].list[d] == null)
                        {
                            ax.Add($"{axis[d]}");
                            isAdd = true;
                        }
                    }
                    tmp += ")";
                    if (isAdd)
                    {
                        error.Add(tmp);    
                    }
                }

                InformationProblem.text = "문제 : " + (error.Count == 0 ? "없음" : string.Join(", ", error));
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
