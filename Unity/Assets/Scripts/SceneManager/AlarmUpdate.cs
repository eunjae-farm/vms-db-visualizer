using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmUpdate : MonoBehaviour
{
    public DatabaseConnector Connector;
    public GameObject Content;
    public GameObject Prefab;

    public float RefreshTick = 5;
    public float currentTick = 5;
    public List<GameObject> g;
    public GameObject Alarm;
    public List<GameObject> Buttones;

    public void Disable()
    {
        Alarm.SetActive(false);
        foreach (var button in Buttones)
        {
            button.SetActive(false);
        }
    }

    public void Enable()
    {
        Alarm.SetActive(true);
        foreach (var button in Buttones)
        {
            button.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        currentTick += Time.deltaTime;

        if (currentTick >= RefreshTick) {
            foreach(var item in g)
            {
                Destroy(item);
            }
            g.Clear();
            currentTick -= 5;

            if (Connector.Alarm != null)
            {
                foreach (var item in Connector.Alarm)
                {
                    GameObject myInstance = Instantiate(Prefab, Content.transform);
                    myInstance.GetComponent<Setting_For_Panel>().SetAlarm(item);
                    g.Add(myInstance);
                }
            }
        }
    }
}
