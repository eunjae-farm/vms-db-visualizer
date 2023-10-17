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

    public void DatabaseConnect()
    {
        GetComponent<Server>().Login();
    }

    IEnumerator Fade()
    {
        while (true)
        {
            Node = GetComponent<Server>().Node()
                .Select(item => item.Name)
                .ToList();

            yield return new WaitForSeconds(5);
        }
    }

    void Start()
    {
        GetComponent<Server>().Login();

        StartCoroutine(Fade());
    }
    void OnDestroy()
    {
        
        GetComponent<Server>().Logout();
    }

    void Update()
    {
        
    }
}

