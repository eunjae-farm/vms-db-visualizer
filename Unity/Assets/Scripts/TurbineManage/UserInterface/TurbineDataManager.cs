using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineDataManager : MonoBehaviour
{
    public TMPro.TMP_InputField InformationName;
    public TMPro.TMP_InputField InformationDBIP;
    public TMPro.TMP_InputField InformationDBName;
    public TMPro.TMP_InputField InformationId;
    public TMPro.TMP_InputField InformationPw;

    public GameObject ContentFromScrollView;
    public GameObject Prefab;

    public GameObject PopupSave;
    public GameObject PopupDelete;



    public void ConnectionTest()
    {
        Debug.Log("Connect Test");
    }
    public void Add()
    {
        Debug.Log("Add");

    }
    public void Delete()
    {
        Debug.Log("Delete");
        PopupDelete.SetActive(true);
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
        //foreach (var item in g)
        //{
        //    Destroy(item);
        //}
        //g.Clear();


        //if (Connector.Alarm != null)
        //{
        //    foreach (var item in Connector.Alarm)
        //    {
        //        GameObject myInstance = Instantiate(Prefab, Content.transform);
        //        myInstance.GetComponent<Setting_For_Panel>().SetAlarm(item);
        //        myInstance.GetComponent<Setting_For_Panel>().MouseClick += AlarmUpdate_MouseClick;
        //        g.Add(myInstance);
        //    }
        //}
    }
}
