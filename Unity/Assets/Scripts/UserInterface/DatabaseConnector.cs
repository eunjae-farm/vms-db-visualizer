using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class DatabaseConnector : MonoBehaviour
{
    public Button Connect;
    public List<string> Node;
    public List<string> Alarm;

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
                .Select(item => $"{item.NodeId}_{item.Name}")
                .ToList();
            Alarm = GetComponent<Server>().Alarm(10, 0)
                .Select(item => $"{item.Date}_{item.Title}_{item.Node}_{item.Status}")
                .ToList();

            t = !t;
            Wind.GetComponent<GeneratorMotion>().OutterBody(t);

            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator GetLatestSearch()
    {

        yield return new WaitForSeconds(1);
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

