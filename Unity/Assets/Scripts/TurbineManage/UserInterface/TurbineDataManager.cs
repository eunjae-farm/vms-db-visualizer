using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TurbineDataManager : MonoBehaviour
{
    [Header("Information")]
    public TMPro.TMP_InputField InformationName;
    public TMPro.TMP_InputField InformationDBIP;
    public TMPro.TMP_InputField InformationDBName;
    public TMPro.TMP_InputField InformationId;
    public TMPro.TMP_InputField InformationPw;
    public TMPro.TMP_InputField InformationWindTurbine;

    [Header("Additional Information")]
    public TMPro.TMP_InputField WingSpeed;
    public TMPro.TMP_InputField SlowSpeed;
    public TMPro.TMP_InputField FastSpeed;
    public TMPro.TMP_InputField Magnitude;
    
    [Header("Seted Turbine List")]
    public GameObject ContentFromScrollView;
    public GameObject Prefab;

    [Header("Popup for adding WindTurbin")]
    public GameObject ContentFromTurbineScrollView;
    public GameObject PrefabForContentFromTurbineScrollView;

    [Header("Popup")]
    public GameObject PopupTurbine;
    public GameObject PopupSave;
    public GameObject PopupDelete;
    public PopupForAlarm Alarm;

    private List<GameObject> contentOfScrollView = new List<GameObject>();
    private List<GameObject> contentOfScrollViewToAddingWindTurbine = new List<GameObject>();

    private int EditTurbineIndex = -1;

    bool ValidateInformation()
    {
        var list = new List<TMPro.TMP_InputField>
        {
            InformationName,
            InformationDBIP,
            InformationDBName,
            InformationId,
            InformationPw,
        };

        var floatList = new List<TMPro.TMP_InputField>
        {
            WingSpeed,
            SlowSpeed,
            FastSpeed,
            Magnitude,
        };

        foreach (var f in floatList)
        {
            if (!float.TryParse(f.text, out _))
            {
                Alarm.Open(PopupForAlarm.ButtonType.Error, "정보의 데이터가 모두 기입이 되지 않았습니다.");
                return false;
            }
        }
        
        foreach (var l in list)
        {
            if (l.text == "")
            {
                Alarm.Open(PopupForAlarm.ButtonType.Error, "정보의 데이터가 모두 기입이 되지 않았습니다.");
                return false;
            }
        }
        return true;
    }

    public void ConnectionTest()
    {
        if (!ValidateInformation())
        {
            return;
        }

        foreach (var item in contentOfScrollViewToAddingWindTurbine)
        {
            Destroy(item);
        }
        contentOfScrollViewToAddingWindTurbine.Clear();
        PopupTurbine.SetActive(true);

        Task.Run(() =>
        {
            var result = new LoginObject();
            result.DatabaseIP = InformationDBIP.text;
            result.DatabaseName = InformationDBName.text;
            result.DatabaseId = InformationId.text;
            result.DatabasePw = InformationPw.text;
            
            if (!Server.Instance.Login(result))
            {
                UnityThread.executeInUpdate(() =>
                {
                    Alarm.Open(PopupForAlarm.ButtonType.Error, "서버 접속에 실패하였습니다.");
                });
                return;
            }
            var node = Server.Instance.Node();
            Debug.Log("load for node data from server");

            UnityThread.executeInUpdate(() =>
            {
                int count = 0;
                for (int i = 0; i < node.Count; i++)
                {
                    //Debug.Log($"{n.NodeType} : {n.Name} : {n}");
                    // node type 1 is magic number for checking turbine.
                    // If you want to check the value of the node type. you can check disassemble to installed vms software in your computer.
                    // I have been disassembled to vms software to early. https://github.com/eunjae-farm/vms-db-visualizer/issues/2
                    if (node[i].NodeType == 1)
                    {
                        var obj = Instantiate(PrefabForContentFromTurbineScrollView, ContentFromTurbineScrollView.transform);
                        obj.GetComponent<WindTurbineElement>().Setup(node[i].NodeId, node[i].Name, $"{node[i].Status}", $"{node[i].Active}", count);
                        obj.GetComponent<WindTurbineElement>().Clicked += TurbineDataManager_Clicked;
                        contentOfScrollViewToAddingWindTurbine.Add(obj);
                        count += 1;
                    }
                }
            });
        });
    }

    private void TurbineDataManager_Clicked(int nodeId, string name)
    {
        InformationWindTurbine.text = $"{nodeId}:{name}";
    }

    public void Add()
    {
        if (!ValidateInformation())
        {
            return;
        }
        var data = InformationWindTurbine.text.Split(":");
        if (data.Length == 1)
        {
            Alarm.Open(PopupForAlarm.ButtonType.Error, "풍력발전기가 선택이되지 않았습니다.");
            return;
        }
        if (!int.TryParse(data[0], out int port))
        {
            return;
        }

        Debug.Log("Add");
        TurbineConnectionDataManager.Instance.Data.Add(new TurbineConnectionData
        {
            DBIP = InformationDBIP.text,
            Name = InformationName.text,
            DBName = InformationDBName.text,
            ID = InformationId.text,
            PW = InformationPw.text,
            NodeId = port,
            NodeName = string.Join(":", data[1..]),
            WingRotatePerSeconds = float.Parse(WingSpeed.text),
            SlowRotateSpeed = float.Parse(SlowSpeed.text),
            FastRotateSpeed = float.Parse(FastSpeed.text),
            MagnitudeForMotion = float.Parse(Magnitude.text)
        });
        Start();
    }

    public void TurbineSelect()
    {
        PopupTurbine.SetActive(false);
    }

    public void TurbineCancel()
    {
        PopupTurbine.SetActive(false);
        InformationWindTurbine.text = "";
    }

    public void Edit()
    {
        if (EditTurbineIndex == -1)
        {
            Alarm.Open(PopupForAlarm.ButtonType.Error, "수정할 데이터를 선택하지 않으셨습니다.");
            return;
        }
        
        if (!ValidateInformation())
        {
            return;
        }
        var data = InformationWindTurbine.text.Split(":");
        if (data.Length == 1)
        {
            Alarm.Open(PopupForAlarm.ButtonType.Error, "풍력발전기가 선택이되지 않았습니다.");
            return;
        }
        
        if (!int.TryParse(data[0], out int port))
        {
            return;
        }

        Debug.Log("Edit");
        TurbineConnectionDataManager.Instance.Data[EditTurbineIndex] = new TurbineConnectionData
        {
            DBIP = InformationDBIP.text,
            Name = InformationName.text,
            DBName = InformationDBName.text,
            ID = InformationId.text,
            PW = InformationPw.text,
            NodeId = port,
            NodeName = string.Join(":", data[1..]),
            WingRotatePerSeconds = float.Parse(WingSpeed.text),
            SlowRotateSpeed = float.Parse(SlowSpeed.text),
            FastRotateSpeed = float.Parse(FastSpeed.text),
            MagnitudeForMotion = float.Parse(Magnitude.text)
        };
        Start();
    }

    public void Delete()
    {
        if (EditTurbineIndex == -1)
        {
            Alarm.Open(PopupForAlarm.ButtonType.Error, "삭제할 데이터를 선택하지 않으셨습니다.");
            return;
        }
        
        Debug.Log("Delete");
        PopupDelete.SetActive(true);
        Start();
    }


    public void PopupDeleteYes()
    {
        TurbineConnectionDataManager.Instance.Data.RemoveAt(EditTurbineIndex);
        EditTurbineIndex = -1;
        PopupDelete.SetActive(false);
        Start();
    }

    public void PopupDeleteNo()
    {
        PopupDelete.SetActive(false);
    }

    public void Save()
    {
        TurbineConnectionDataManager.Instance.Save();
        Alarm.Open(PopupForAlarm.ButtonType.Default, "성공적으로 데이터를 저장하였습니다.");
    }
    public void Back()
    {
        Debug.Log("Back");
        PopupSave.SetActive(true);
    }

    public void PopupSaveYes()
    {
        PopupSave.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Visualizer");
    }
    public void PopupSaveNo()
    {
        PopupSave.SetActive(false);
    }


    private void Awake()
    {
        TurbineConnectionDataManager.Instance.Load();
    }

    private void Start()
    {
        foreach (var item in contentOfScrollView)
        {
            Destroy(item);
        }
        contentOfScrollView.Clear();

        // shallow copy, because Data type is List type based class.
        var data = TurbineConnectionDataManager.Instance.Data;
        if (data.Count != 0)
        {
            for (int i = 0; i < data.Count; i++)
            {
                GameObject myInstance = Instantiate(Prefab, ContentFromScrollView.transform);
                myInstance.GetComponent<ElementCode>().Setup(data[i].Name, data[i].NodeName, i);
                myInstance.GetComponent<ElementCode>().Click += TurbineDataManager_Click;
                contentOfScrollView.Add(myInstance);
            }
        }
    }

    private void TurbineDataManager_Click(string alias_name, string turbine_name, int idx)
    {
        var data = TurbineConnectionDataManager.Instance.Data[idx];
        InformationName.text = data.Name;
        InformationDBIP.text = data.DBIP;
        InformationDBName.text = data.DBName;
        InformationId.text = data.ID;
        InformationPw.text = data.PW;
        InformationWindTurbine.text = $"{data.NodeId}:{data.NodeName}";
        WingSpeed.text = data.WingRotatePerSeconds.ToString();
        SlowSpeed.text = data.SlowRotateSpeed.ToString();
        FastSpeed.text = data.FastRotateSpeed.ToString();
        Magnitude.text = data.MagnitudeForMotion.ToString();
        
        EditTurbineIndex = idx;
    }
}
