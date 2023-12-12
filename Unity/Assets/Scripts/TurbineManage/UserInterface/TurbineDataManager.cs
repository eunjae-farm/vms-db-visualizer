using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TurbineDataManager : MonoBehaviour
{
    public TMPro.TMP_InputField InformationName;
    public TMPro.TMP_InputField InformationDBIP;
    public TMPro.TMP_InputField InformationDBName;
    public TMPro.TMP_InputField InformationId;
    public TMPro.TMP_InputField InformationPw;
    public TMPro.TMP_InputField InformationWindTurbine;

    public GameObject ContentFromScrollView;
    public GameObject Prefab;

    public GameObject PopupSave;
    public GameObject PopupDelete;

    public PopupForAlarm Alarm;

    private List<GameObject> contentOfScrollView = new List<GameObject>();

    public void ConnectionTest()
    {
        Task.Run(() =>
        {
            Server.Instance.Login(new LoginObject());
            var node = Server.Instance.Node();
            Debug.Log("load for node data from server");
            
            foreach(var n in node)
            {
                Debug.Log($"{n.NodeType} : {n.Name} : {n}");
            }
        });
    }

    public void Add()
    {
        Debug.Log("Add");
    }

    public void Delete()
    {
        Alarm.Open(PopupForAlarm.ButtonType.Error, "삭제할 데이터를 선택하지 않으셨습니다.");
        Debug.Log("Delete");
        //PopupDelete.SetActive(true);
    }
    public void Edit()
    {
        Debug.Log("Edit");

    }
    public void Back()
    {
        Debug.Log("Back");
        PopupSave.SetActive(true);
    }

    public void PopupSaveYes()
    {
        PopupSave.SetActive(false);
        TurbineConnectionDataManager.Instance.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Visualizer");
    }
    public void PopupSaveNo()
    {
        PopupSave.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Visualizer");
    }

    public void PopupDeleteYes()
    {
        PopupDelete.SetActive(false);
    }

    public void PopupDeleteNo()
    {
        PopupDelete.SetActive(false);
    }

    private void Start()
    {
        TurbineConnectionDataManager.Instance.Load();

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
        InformationWindTurbine.text = $"{data.NodeName}:${data.NodeId}";
        Server.Instance.Login(new LoginObject());
    }
}
