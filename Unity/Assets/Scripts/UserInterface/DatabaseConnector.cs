using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class DatabaseConnector : MonoBehaviour
{
    public List<VMSNode> Node;
    public List<VMSAlarmWithNode> Alarm;

    public List<NodeData> CurrentData;

    public GameObject Wind;

    public void DatabaseConnect()
    {
        
    }

    void UpdateNode()
    {
        Wind.GetComponent<GeneratorMotion>().OutterBody(false);
        
        Task.Run(() =>
        {
            while (true)
            {
                Server.Instance.Login(new LoginObject());
                Node = Server.Instance.Node()
                    //.Select(item => $"{item.NodeId}_{item.Name}")
                    .ToList();

                var www = Node.Select(item => item.Name).ToList();
                Debug.Log(string.Join("\n", www.ToArray()));
                
                Alarm = Server.Instance.Alarm(100, 0)
                    .Select(item => new VMSAlarmWithNode(item, Node.First(i => i.NodeId == item.Node).Name))
                    //.Select(item => $"{item.Date}_{item.Title}_{item.Node}_{item.Status}")
                    .ToList();

                var s = new System.Diagnostics.Stopwatch();
                s.Start();

                var p = Node
                    .AsParallel()
                    .Where(node => !node.Name.ToLower().Contains("low"))
                    .Where(node => !node.Name.ToLower().Contains("high"))
                    .Where(node => !node.Name.ToLower().Contains("env"))
                    .Where(node => !node.Name.ToLower().Contains("polar"))
                    .Where(node => !node.Name.ToLower().Contains("hz"))
                    .Select(node => (node, Server.Instance.Search(node.NodeId, 1, 0)))
                    .Where(item => item.Item2.Any())
                    .Select(item =>
                    {
                        return item;
                    })
                    .Select(item => (item.node, item.Item2.First()))
                    .Select(item => (item.node,
                                        search: item.Item2,
                                        fft: Server.Instance.fft(item.Item2.Id, item.Item2.TimeSignalLines, (int)item.Item2.EndFrequency),
                                        charts: Server.Instance.Charts(item.Item2.Id, (int)item.Item2.SampleRate)))
                    //.Where(item => item.fft != null && item.charts != null)
                    .Select(item => new NodeData(item.node, item.fft, item.charts, item.search))
                    .OrderBy(item => item.Node.Name)
                    .ToList();

                CurrentData = p;
                s.Stop();
                Debug.Log(s.ElapsedMilliseconds);

                UnityThread.executeInLateUpdate(() =>
                {
                    Wind.GetComponent<GeneratorMotion>().OutterBody(true);
                    Wind.GetComponent<GeneratorMotion>().SetData(CurrentData);
                });

                Task.Delay(3600 * 1000).Wait();
            }
        });
    }

    void Start()
    {
        UpdateNode();

    }

    private void OnApplicationQuit()
    {
        Server.Instance.Logout();
    }

    void Update()
    {
        
    }
}

