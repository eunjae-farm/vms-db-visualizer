using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OverviewTurbineSelect : SceneManager
{
    public GameObject TerrainOfOverview;
    public GameObject TerrainOfNercel;
    
    public TMPro.TMP_Text NameTag;
    
    public TMPro.TMP_Text TextOfTemperture;
    public TMPro.TMP_Text TextOfDecibel;
    public TMPro.TMP_Text TextOfDust;
    
    private int index = 0;
    public GameObject AlarmComponent;
    private List<GameObject> AlarmComponentInListView = new List<GameObject>();
    public GameObject ContentOfListView;
    public GameObject WindTurbines;

    [SerializeField]
    private AnimationCurve moveAnimationCurve;

    [SerializeField]
    private float moveDuration;
    private float currentMoveDuration = 0;
    private int moveDirection = 0; // -1 <<, 1 >> 

    public CameraSceneMove CameraManager;
    public MoreDetailTurbine DataThrou;
    public GameObject UI;
    public PopupForAlarm PopupAlarm;

    public override void Enable(bool tomain)
    {
        UI.SetActive(true);
        TerrainOfOverview.SetActive(true);
        TerrainOfNercel.SetActive(false);
    }

    public override void Disable(bool tomain)
    {
        UI.SetActive(!tomain);
        TerrainOfOverview.SetActive(false);
        TerrainOfNercel.SetActive(true);
    }


    private void Update()
    {
        if (moveDirection != 0)
        {
            if (TurbineConnectionDataManager.Instance.Data.Count == 1)
            {
                moveDirection = 0;
                return;
            }
            
            currentMoveDuration += Time.deltaTime;

            if (currentMoveDuration >= moveDuration)
            {
                currentMoveDuration = moveDuration;
            }

            var evals = moveAnimationCurve.Evaluate(currentMoveDuration / moveDuration);
            bool isThreshold = false;
            if (evals > 0.5)
            {
                isThreshold = true;
            }

            var item = WindTurbines;
            // convert to 0 1 2 => 1, 0, -1
            //-1 0 1 
            float x = -18;
            x -= 20 * evals * moveDirection;
            if (isThreshold)
            {
                x -= -moveDirection * 20;
            }

            // -18 -> -8
            item.transform.position = new Vector3(x, item.transform.position.y, item.transform.position.z);


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
        
        currentNode = null;
        currentAlarm = null;

        WindTurbines.GetComponent<RotateTurbine>().SlowRotateSpeed = d.SlowRotateSpeed;
        WindTurbines.GetComponent<RotateTurbine>().FastwRotateSpeed = d.FastRotateSpeed;
        WindTurbines.GetComponent<RotateTurbine>().WingRotatePerSeconds = d.SlowRotateSpeed;
        WindTurbines.GetComponent<RandomGeneratorMotion>().MagnOfCorrect = d.MagnitudeOfCorrectForMotion;
        WindTurbines.GetComponent<RandomGeneratorMotion>().MagnOfError = d.MagnitudeOfErrorForMotion;
        
        foreach (var item in AlarmComponentInListView)
        {
            item.SetActive(false);
        }
        
        UpdateForView();
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

    List<VMSNode> currentNode = null;
    List<VMSAlarmWithNode> currentAlarm = null;

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

            var env = Server.Instance.Environment();
            
            var data = Get();
            currentNode = Server.Instance.Node(turbine.NodeId)
                .Where(node => data.ObserveBearing.Select(item => item.ToLower())
                    .Contains(node.Name.ToLower()))
                .ToList();
            
            currentAlarm = Server.Instance.Alarm(100, 0, currentNode.Select(item => item.NodeId).ToList())
                    .Select(item => (item: item, name: currentNode.FirstOrDefault(i => i.NodeId == item.Node)?.Name))
                    .Where(item => item.name != null)
                    .Select(item => new VMSAlarmWithNode(item.item, item.name))
                    //.Select(item => $"{item.Date}_{item.Title}_{item.Node}_{item.Status}")
                    .ToList();

            UnityThread.executeInUpdate(() =>
            {
                TextOfDust.text = $"먼지 농도 : {env.Dust:F1} \u338d/m\u00b3";
                TextOfDecibel.text = $"소음         : {env.Sound:F1} dB";
                TextOfTemperture.text = $"온도         : {env.Temp:F1} \u00b0C";
                
                for (int i = AlarmComponentInListView.Count; i < currentAlarm.Count; i++)
                {
                    GameObject myInstance = Instantiate(AlarmComponent, ContentOfListView.transform);
                    myInstance.GetComponent<Setting_For_Panel>().MouseClick += OverviewTurbineSelect_MouseClick;
                    AlarmComponentInListView.Add(myInstance);
                }

                for (int i = 0; i < currentAlarm.Count; i++)
                {
                    AlarmComponentInListView[i].GetComponent<Setting_For_Panel>().SetAlarm(currentAlarm[i]);
                    AlarmComponentInListView[i].SetActive(true);
                }

                for (int i = currentAlarm.Count; i < AlarmComponentInListView.Count; i++)
                {
                    AlarmComponentInListView[i].SetActive(false);
                }
            });
        });
    }

    
    public void SelectButton()
    {
        Debug.Log("SelectButton");
        if (currentNode == null || currentAlarm == null)
        {
            PopupAlarm.AutoClose = true;
            PopupAlarm.Open(PopupForAlarm.ButtonType.Error, "먼저 알람 정보를 불러와주세요.");
            return;
        }

        CameraManager.SetCamera(1, true);
        DataThrou.LoadForLatestTime(Get(), currentNode);
        DataThrou.gameObject.GetComponent<SemiTurbineManager>().SetTurbineIndex(this.index);
    }
    
    private void OverviewTurbineSelect_MouseClick(GameObject arg1, VMSAlarmWithNode arg2)
    {
        if (currentNode == null || currentAlarm == null)
        {
            PopupAlarm.AutoClose = true;
            PopupAlarm.Open(PopupForAlarm.ButtonType.Error, "먼저 알람 정보를 불러와주세요.");
            return;
        }
        
        CameraManager.SetCamera(1, true);
        var date = DateTimeOffset.Parse(arg2.Date, CultureInfo.InvariantCulture).UtcDateTime;
        DataThrou.LoadForAlarm(Get(), currentNode, arg2);
        DataThrou.gameObject.GetComponent<SemiTurbineManager>().SetTurbineIndex(this.index);
    }
}
