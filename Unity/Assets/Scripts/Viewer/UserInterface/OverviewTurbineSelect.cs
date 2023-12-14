using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OverviewTurbineSelect : MonoBehaviour
{
    public TMPro.TMP_Text NameTag;
    private int index = 0;
    public GameObject AlarmComponent;
    private List<GameObject> AlarmComponentInListView = new List<GameObject>();
    public GameObject ContentOfListView;

    private void Load()
    {
        TurbineConnectionDataManager.Instance.Load();
        index = 0;
        Set(index);
    }
    private void Set(int index)
    {
        this.index += index;
        if (TurbineConnectionDataManager.Instance.Data.Count == 0)
        {
            NameTag.text = $"설정된 발전기 정보 없음";
            return;
        }

        if (this.index < 0)
        {
            this.index = TurbineConnectionDataManager.Instance.Data.Count - 1;
        }

        var d = TurbineConnectionDataManager.Instance.Data[this.index % TurbineConnectionDataManager.Instance.Data.Count];

        NameTag.text = $"풍력발전기 : {d.Name}";

        
    }

    private TurbineConnectionData Get()
    {
        if (TurbineConnectionDataManager.Instance.Data.Count == 0)
        {
            return null;
        }

        return TurbineConnectionDataManager.Instance.Data[this.index % TurbineConnectionDataManager.Instance.Data.Count];
    }

    private void Start()
    {
        Load();

    }


    public void PreviousButton()
    {
        Debug.Log("Previous");
        Set(-1);
    }

    public void NextButton()
    {
        Debug.Log("NextButton");
        Set(+1);
    }

    public void SelectButton()
    {
        Debug.Log("SelectButton");
    }

    public void ManageButton()
    {
        Debug.Log("AddButton");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/TurbineManager");
    }

    public void AlarmButton()
    {
        Debug.Log("DeleteButton");
        UpdateForView();
    }


    void UpdateForView()
    {
        Task.Run(() =>
        {
            var turbine = Get();
            if (turbine == null)
            {
                return;
            }
            
            Server.Instance.Login(new LoginObject
            {
                DatabaseIP = turbine.DBIP,
                DatabaseName = turbine.DBName,
                DatabaseId = turbine.ID,
                DatabasePw = turbine.PW
            });

            var Node = Server.Instance.Node();
            var Alarm = Server.Instance.Alarm(100, 0)
                    .Select(item => new VMSAlarmWithNode(item, Node.First(i => i.NodeId == item.Node).Name))
                    //.Select(item => $"{item.Date}_{item.Title}_{item.Node}_{item.Status}")
                    .ToList();

            UnityThread.executeInUpdate(() =>
            {

                for (int i = AlarmComponentInListView.Count; i < Alarm.Count; i++)
                {
                    GameObject myInstance = Instantiate(AlarmComponent, ContentOfListView.transform);
                    myInstance.GetComponent<Setting_For_Panel>().MouseClick += OverviewTurbineSelect_MouseClick;
                    AlarmComponentInListView.Add(myInstance);
                }

                for (int i = 0; i < Alarm.Count; i++)
                {
                    AlarmComponentInListView[i].GetComponent<Setting_For_Panel>().SetAlarm(Alarm[i]);
                    AlarmComponentInListView[i].SetActive(true);
                }

                for (int i = Alarm.Count; i < AlarmComponentInListView.Count; i++)
                {
                    AlarmComponentInListView[i].SetActive(false);
                }
            });

            Server.Instance.Logout();
        });
    }

    private void OverviewTurbineSelect_MouseClick(GameObject arg1, VMSAlarmWithNode arg2)
    {

    }
}
