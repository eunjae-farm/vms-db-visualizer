using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class MoreDetailTurbine : SceneManager
{
    public GameObject UI;
    
    public CameraSceneMove Cameras;
    public PopupForAlarm PopupAlarm;
    public RandomGeneratorMotion TurbineMotion;
    public DrawChartsManager ChartManager;

    public TMP_Text InformationDateTime;
    public TMP_Text InformationDeviceStatus;
    public TMP_Text InformationProblem;
    
    public List<Button> CamButton;
    public Color ActivatedCamColor;
    public Color DefaultCamColor;
    public PostProcessVolume Volume;
    
    public void BackToMain()
    {
        // 뒤로가기 눌렀을 때,
        // 1. Outline 삭제
        // 다시 커버 닫기
        // 그 외 모든 UI 정리하기
        TurbineMotion.gameObject.transform.position = new Vector3(-18.3f, 54.3f, 394.8f);
        TurbineMotion.gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        
        TurbineMotion.OutterBody(false);
        TurbineMotion.OnOutline(-1, false);
        for (int c = 0; c < CamButton.Count; c++)
        {
            CamButton[c].GetComponent<Image>().color = (0 == c ? ActivatedCamColor : DefaultCamColor);
        }

        Volume.profile.GetSetting<DepthOfField>().focusDistance.value = 20.0f;
        Volume.profile.GetSetting<DepthOfField>().aperture.value = 3.6f;
        Volume.profile.GetSetting<DepthOfField>().focalLength.value = 225f;
                Volume.profile.GetSetting<DepthOfField>().kernelSize.value = KernelSize.Small;

        Cameras.SetCamera(0, true);
    }

    public void SetCam(int i)
    {
        for (int c = 0; c < CamButton.Count; c++)
        {
            CamButton[c].GetComponent<Image>().color = ((i - 1) == c ? ActivatedCamColor : DefaultCamColor);
        }
        switch (i)
        {
            case 1:
                Volume.profile.GetSetting<DepthOfField>().focusDistance.value = 2.35f;
                Volume.profile.GetSetting<DepthOfField>().aperture.value = 32f;
                Volume.profile.GetSetting<DepthOfField>().focalLength.value = 120f;
                Volume.profile.GetSetting<DepthOfField>().kernelSize.value = KernelSize.Small;
                break;
            case 2:
                Volume.profile.GetSetting<DepthOfField>().focusDistance.value = 2.0f;
                Volume.profile.GetSetting<DepthOfField>().aperture.value = 32f;
                Volume.profile.GetSetting<DepthOfField>().focalLength.value = 115f;
                Volume.profile.GetSetting<DepthOfField>().kernelSize.value = KernelSize.Small;
                break;
            case 3:
                Volume.profile.GetSetting<DepthOfField>().focusDistance.value = 2.35f;
                Volume.profile.GetSetting<DepthOfField>().aperture.value = 32f;
                Volume.profile.GetSetting<DepthOfField>().focalLength.value = 120f;
                Volume.profile.GetSetting<DepthOfField>().kernelSize.value = KernelSize.Large;
                break;
            case 4:
                Volume.profile.GetSetting<DepthOfField>().focusDistance.value = 6f;
                Volume.profile.GetSetting<DepthOfField>().aperture.value = 13f;
                Volume.profile.GetSetting<DepthOfField>().focalLength.value = 120f;
                Volume.profile.GetSetting<DepthOfField>().kernelSize.value = KernelSize.Small;
                break;
        }
        if (i == 4)
        {
        }else
        {
        }
        Cameras.SetCamera(i, false);
    }
    
    public override void Enable(bool tomain)
    {
        UI.SetActive(true);
    }

    public override void Disable(bool tomain)
    {
        UI.SetActive(!tomain);
    }
    
    public List<NodeData> nodeData { get; private set; }
    public TurbineConnectionData turbineConnection;
    public  List<VMSNode> vmsNode;
    public List<VMSAlarmWithNode> vmsAlarm;

    private void InitSetup()
    {
        SetCam(4);
        TurbineMotion.gameObject.transform.position = new Vector3(0,0,0);
        TurbineMotion.gameObject.transform.localScale = new Vector3(1,1,1);
        PopupAlarm.AutoClose = false;
        PopupAlarm.Open(PopupForAlarm.ButtonType.Warring, "데이터 로딩중");
        TurbineMotion.OutterBody(false);
    }

    public void LoadForAlarm(TurbineConnectionData data, List<VMSNode> node, VMSAlarmWithNode alarm,
        float seconds = 600)
    {
        var point = DateTimeOffset.Parse(alarm.Date, CultureInfo.InvariantCulture).UtcDateTime;
        LoadForSelectTime(data, node, point, seconds);
    }

    public void LoadForSelectTime(TurbineConnectionData data, List<VMSNode> node, DateTime point, float seconds = 600)
    {
        InitSetup();
        Task.Run(() =>
        {
            var s = new System.Diagnostics.Stopwatch();
            s.Start();

            var p = node.AsParallel()
                .Where(node => data.ObserveBearing.Select(item => item.ToLower())
                    .Contains(node.Name.ToLower()))
                .Select(node => (node, search: Server.Instance.Search(
                    node.NodeId,
                    point.AddSeconds(-seconds),
                    point.AddSeconds(seconds))))
                .Where(item => item.search != null && item.search.Any())
                .Select(item => (node: item.node, search: item.search.First()))
                .Select(item => (item.node,
                    search: item.search,
                    fft: Server.Instance.fft(item.Item2.Id, item.Item2.TimeSignalLines,
                        (int)item.Item2.EndFrequency),
                    charts: Server.Instance.Charts(item.Item2.Id, (int)item.Item2.SampleRate)))
                .Where(item => item.fft != null && item.charts != null)
                .Select(item => new NodeData(item.node, item.fft, item.charts, item.search))
                .OrderBy(item => item.Node.Name)
                .ToList();

            LoadForVibData(data, node, p, point, seconds);

        });
    }
    
    public void LoadForLatestTime(TurbineConnectionData data, List<VMSNode> node)
    {
        InitSetup();
        Task.Run(() =>
        {
            // var s = new System.Diagnostics.Stopwatch();
            // 데이터 파싱 과정
            var p = node.AsParallel()
                .Where(node => data.ObserveBearing.Select(item => item.ToLower())
                    .Contains(node.Name.ToLower()))
                .Select(node => (node, search: Server.Instance.Search(node.NodeId, 1, 0)))
                .Where(item => item.search != null && item.search.Any())
                .Select(item => (node: item.node, search: item.search.First()))
                .Select(item => (item.node,
                    search: item.search,
                    fft: Server.Instance.fft(item.Item2.Id, item.Item2.TimeSignalLines, (int)item.Item2.EndFrequency),
                    charts: Server.Instance.Charts(item.Item2.Id, (int)item.Item2.SampleRate)))
                .Where(item => item.fft != null && item.charts != null)
                .Select(item => new NodeData(item.node, item.fft, item.charts, item.search))
                .OrderBy(item => item.Node.Name)
                .ToList();

            var point = DateTimeOffset.Parse(p.OrderBy(n => n.Node.Name).First().Search.Date, CultureInfo.InvariantCulture).UtcDateTime;

            LoadForVibData(data, node, p, point);
        });
    }

    public void LoadForVibData(TurbineConnectionData data, List<VMSNode> node, List<NodeData> nodeData, DateTime point, float seconds = 60)
    {
        // 파싱된 데이터들의 정확한 시간들
        var time = nodeData.Select(item => item.Search.Date)
            .Select(item => DateTimeOffset.Parse(item, CultureInfo.InvariantCulture).UtcDateTime)
            .OrderBy(item => item)
            .ToList();
        
        // 알림 정보를 수집하기 위한 노드 정보 
        var nodeIds = node.AsParallel()
            .Where(node => data.ObserveBearing.Select(item => item.ToLower())
                .Contains(node.Name.ToLower()))
            .Select(item => item.NodeId)
            .ToList();
        
        if (time.Count == 0)
        {
            UnityThread.executeInUpdate(() =>
            {
                InformationDateTime.text = "날짜 : " + "데이터를 찾을 수 없습니다.";
                InformationDeviceStatus.text = "장비 상태 : 데이터 없음";
                InformationProblem.text = "누락 문제 : 데이터 없음";
            });
            return;
        }

        var alarms = Server.Instance.Alarm(point.AddSeconds(-seconds), point.AddSeconds(seconds), nodeIds);
        alarms = alarms.DistinctBy(i => i.Node).ToList();
        
        string deviceName = "";
        
        for (int i = 0; i < alarms.Count; i++)
        {
            var name = node.First(id => alarms[i].Node == id.NodeId).Name;
            deviceName += $"{name}({alarms[i].GetStatus()}), ";
            // var date = DateTime.Parse(alarms[i].Date);
        }
        deviceName = deviceName.TrimEnd(", ");
        
        UnityThread.executeInUpdate(() =>
        {
            if (alarms.Count != 0)
            {
                InformationDeviceStatus.text = $"장비 상태 : {deviceName}";
            }
            else
            {
                InformationDeviceStatus.text = $"장비 상태 : 알림 없음";
            }

            InformationDateTime.text = "날짜 : " + point.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");

            PopupAlarm.Close();
            
            var dev = new List<string>
            {
                "MB RS",
                "GB RS",
                "GB GS",
                "GE DE",
                "GE NDE",
            };
            var axis = "HVA";
            var setData = new List<UnityList<bool>>();
            var overAllData = new List<UnityList<double>>();
            var error = new List<string>();
            
            for (int type = 0; type < 5; type++)
            {
                // var t = new UnityList<bool>();
                var at = new UnityList<bool>();
                string tmp = $"{dev[type]} (";
                bool isAdd = false;
                var ax = new List<string>();
                var oad = new UnityList<double>();
                
                for (int axiss = 0; axiss < 3; axiss++)
                {
                    var id = node.FirstOrDefault(id => id.Name == data.ObserveBearing[type * 3 + axiss]);
                    if (id == null)
                    {
                        PopupAlarm.Open(PopupForAlarm.ButtonType.Error, "모든 축의 정보가 옳바르지 않습니다. 다시 한번 확인해주세요.");
                        return;
                    }
                    var exists = nodeData.Exists(data => data.Node.NodeId == id.NodeId);
                    var aalarm = alarms.Exists(alarm => alarm.Node == id.NodeId);
                    
                        
                    if (!exists)
                    {
                        ax.Add($"{axis[axiss]}");
                        isAdd = true;
                        oad.list.Add(0);
                    }else
                    {
                        // 문제! 이름이 같다면 HVA 가 같다만 문제가 생김!
                        //nodeData.Where(data => data.Node.NodeId == id.NodeId)
                        //    .ToList()
                        //    .ForEach(i => i.Axis.Add(axiss));
                        oad.list.Add(nodeData.First(o => o.Node.NodeId == id.NodeId).Search.Value);
                    }
                    
                    // t.list.Add(exists);
                    at.list.Add(aalarm);
                }
                
                if (isAdd)
                {
                    tmp += string.Join(", ", ax) + ")";
                    error.Add(tmp);
                }

                overAllData.Add(oad);
                setData.Add(at);
            }
            // XYZ
            // HVA
            
            TurbineMotion.SetData(setData, overAllData);
            ChartManager.Setup(nodeData, turbineConnection);

            InformationProblem.text = "누락 문제 : " + (error.Count == 0 ? "없음" : string.Join(", ", error));
        });


        this.nodeData = nodeData;
        turbineConnection = data;
        vmsNode = node;
        // vmsAlarm = alarms;
    }

    private void Update()
    {
        if (!UI.activeSelf)
        {
            return;
        }
        
        //마우스 클릭시
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("this object is ui");
                return;
            }
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
                            
                            if (t.gameObject.layer == 5 || t.gameObject.name.Contains("Terrain"))
                            {
                                return;
                            }

                            while (t.gameObject.name.Split("_")[0] != "00000") {
                                t = t.parent.transform;
                            }
                            int id = int.Parse(t.gameObject.name.Split("_")[1]);
                            if (id == 0)
                            {
                                TurbineMotion.OutterBody(true);
                            }
                            ChartManager.ShowEvent(id, t.gameObject.name);
                            
                            Debug.Log(t.gameObject.name);
                        }
                    }
                }
            }
        }
    }
}
