using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverviewManager : SceneManager
{
    public GameObject Images;
    public GameObject Alarm;
    public Button Replay;
    public Button CurrentView;


    public override void Disable()
    {
        Images.SetActive(true);
        Alarm.GetComponent<AlarmUpdate>().Disable();
        Alarm.SetActive(false);
        Replay.gameObject.SetActive(false);
        CurrentView.gameObject.SetActive(false);
    }

    public override void Enable()
    {
        Images.SetActive(false);
        Alarm.GetComponent<AlarmUpdate>().Enable();
        Alarm.SetActive(true);
        Replay.gameObject.SetActive(true);
        CurrentView.gameObject.SetActive(true);
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
