using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewManager : SceneManager
{
    public GameObject Alarm;


    public override void Disable()
    {
        Alarm.GetComponent<AlarmUpdate>().Disable();
        Alarm.SetActive(false);
    }

    public override void Enable()
    {
        Alarm.GetComponent<AlarmUpdate>().Enable();
        Alarm.SetActive(true);
    }



    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
