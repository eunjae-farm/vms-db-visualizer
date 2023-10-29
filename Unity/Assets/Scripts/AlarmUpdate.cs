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
    // Start is called before the first frame update
    void Start()
    {
    
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

            foreach (var item in Connector.Alarm)
            {
                GameObject myInstance = Instantiate(Prefab, Content.transform);
                myInstance.GetComponent<Setting_For_Panel>().SetAlarm(item);
                g.Add(myInstance);
            }

            currentTick -= 5;
        }
    }
}
