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
        
        GetComponent<Server>().Logout();
    }

    void Update()
    {
        
    }
}

