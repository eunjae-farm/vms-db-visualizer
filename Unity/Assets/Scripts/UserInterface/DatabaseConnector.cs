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
        GetComponent<Server>().Login();
    }

    bool t = false;
    IEnumerator UpdateNode()
    {
        while (true)
        {
            Node = GetComponent<Server>().Node()
                //.Select(item => $"{item.NodeId}_{item.Name}")
                .ToList();
            Alarm = GetComponent<Server>().Alarm(100, 0)
                .Select(item => new VMSAlarmWithNode(item, Node.First(i => i.NodeId == item.Node).Name))
                //.Select(item => $"{item.Date}_{item.Title}_{item.Node}_{item.Status}")
                .ToList();

            t = !t;
            Wind.GetComponent<GeneratorMotion>().OutterBody(t);

            yield return new WaitForSeconds(3600);
        }
    }

    void Start()
    {
        GetComponent<Server>().Login();
        StartCoroutine(UpdateNode());
    }
    void OnDestroy()
    {
        
        GetComponent<Server>().Logout();
    }

    void Update()
    {
        
    }
}

