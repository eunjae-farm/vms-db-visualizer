using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OverviewTurbineSelect : SceneManager
{
    public TMPro.TMP_Text NameTag;
    private int index = 0;
    public GameObject AlarmComponent;
    private List<GameObject> AlarmComponentInListView = new List<GameObject>();
    public GameObject ContentOfListView;
    public List<GameObject> WindTurbines;

    [SerializeField]
    private AnimationCurve moveAnimationCurve;

    [SerializeField]
    private float moveDuration;
    private float currentMoveDuration = 0;
    private int moveDirection = 0; // -1 <<, 1 >> 

    public GameObject UI;

    public override void Enable()
    {
        UI.SetActive(true);
    }

    public override void Disable()
    {
        UI.SetActive(false);
    }


    private void Update()
    {
        if (moveDirection != 0)
        {
            currentMoveDuration += Time.deltaTime;

            if (currentMoveDuration >= moveDuration)
            {
                currentMoveDuration = moveDuration;
            }

            var evals = moveAnimationCurve.Evaluate(currentMoveDuration / moveDuration);
            bool isThreshold = false;
            if (evals > 0.4)
            {
                isThreshold = true;
            }
            for (int i = 0; i < WindTurbines.Count; i++)
            {
                var item = WindTurbines[i];
                // convert to 0 1 2 => 1, 0, -1
                //-1 0 1 
                float x = 300 * -(i - 1);
                x += 300 * evals * moveDirection;
                if (isThreshold)
                {
                    x += -moveDirection * 300;
                }

                item.transform.position = new Vector3(x, 0, 0);
            }

            if (Mathf.Abs(currentMoveDuration - moveDuration) < float.Epsilon)
            {
                moveDirection = 0;
                currentMoveDuration = 0;
            }
        }
    }

    private void Load()
    {
        TurbineConnectionDataManager.Instance.Load();
        index = 0;
        Set(index);
    }

    private void Set(int index)
    {
        if (moveDirection != 0)
        {
            return;
        }

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

        moveDirection = index;
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
