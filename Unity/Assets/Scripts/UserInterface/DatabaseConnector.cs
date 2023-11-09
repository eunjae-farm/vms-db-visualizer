using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class DatabaseConnector : MonoBehaviour
{
    public List<VMSNode> Node;
    public List<VMSAlarmWithNode> Alarm;

    public GameObject Wind;

    public void DatabaseConnect()
    {
        
    }

    IEnumerator UpdateNode()
    {
        while (true)
        {
            Node = Server.Instance.Node()
                //.Select(item => $"{item.NodeId}_{item.Name}")
                .ToList();
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
                .Select(node => (node, Server.Instance.Search(node.NodeId, 1, 0)))
                .Where(item => item.Item2.Any())
                
                .Select(item =>
                {
                    Debug.Log(item.node.Name);
                    return item;
                })
                .Select(item => (item.node, item.Item2.First()))
                .Select(item => (item.node,
                                    search: item.Item2,
                                    fft: Server.Instance.fft(item.Item2.Id, item.Item2.TimeSignalLines, (int)item.Item2.EndFrequency),
                                    charts: Server.Instance.Charts(item.Item2.Id, (int)item.Item2.SampleRate)))
                .ToList();
            s.Stop();

            Debug.Log(s.ElapsedMilliseconds + "ms");
            Debug.Log(s.ElapsedMilliseconds + "ms");
            Debug.Log(s.ElapsedMilliseconds + "ms");
            Debug.Log(s.ElapsedMilliseconds + "ms");


            //Wind.GetComponent<GeneratorMotion>().OutterBody(t);

            yield return new WaitForSeconds(3600);
        }
    }

    void Start()
    {
        Server.Instance.Login(new LoginObject());
        StartCoroutine(UpdateNode());
    }

    void OnDestroy()
    {
        Server.Instance.Logout();
    }

    void Update()
    {
        
    }
}

