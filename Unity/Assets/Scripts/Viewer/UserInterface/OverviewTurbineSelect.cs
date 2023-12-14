using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewTurbineSelect : MonoBehaviour
{
    public TMPro.TMP_Text NameTag;
    private int index = 0;

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
    }
}
