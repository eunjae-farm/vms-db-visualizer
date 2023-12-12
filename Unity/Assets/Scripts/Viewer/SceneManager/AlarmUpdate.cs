using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmUpdate : MonoBehaviour
{
    public DatabaseConnector Connector;
    public GameObject Content;
    public GameObject Prefab;

    public float RefreshTick = 5;
    public List<GameObject> g;
    public GameObject Alarm;

    private WaitForSeconds waitSeconds;

    public void Disable()
    {
        Alarm.SetActive(false);
    }

    public void Enable()
    {
        Alarm.SetActive(true);
    }

    public void Start()
    {
        waitSeconds = new WaitForSeconds(RefreshTick);
        StartCoroutine(UpdateForView());
    }

    IEnumerator UpdateForView()
    {
        while(true)
        {
            foreach (var item in g)
            {
                Destroy(item);
            }
            g.Clear();


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
            yield return waitSeconds;
        }
    }


    private void AlarmUpdate_MouseClick(GameObject self, VMSAlarmWithNode data)
    {
    }

}
