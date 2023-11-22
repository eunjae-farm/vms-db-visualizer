using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmUpdate : MonoBehaviour
{
    public DatabaseConnector Connector;
    public GameObject Content;
    public GameObject Prefab;

    public float RefreshTick = 1800;
    public float currentTick = 1800;
    public List<GameObject> g;
    public GameObject Alarm;

    public void Disable()
    {
        Alarm.SetActive(false);
    }

    public void Enable()
    {
        Alarm.SetActive(true);
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
            currentTick -= RefreshTick;

            if (Connector.Alarm != null)
            {
                foreach (var item in Connector.Alarm)
                {
                    GameObject myInstance = Instantiate(Prefab, Content.transform);
                    myInstance.GetComponent<Setting_For_Panel>().SetAlarm(item);
                    myInstance.GetComponent<Setting_For_Panel>().MouseClick += AlarmUpdate_MouseClick;
                    g.Add(myInstance);
                }
            }
        }
    }

    private void AlarmUpdate_MouseClick(GameObject self, VMSAlarmWithNode data)
    {
    }

}
