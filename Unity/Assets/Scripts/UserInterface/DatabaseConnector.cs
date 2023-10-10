using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DatabaseConnector : MonoBehaviour
{
    public TMPro.TMP_InputField DatabaseIP;
    public TMPro.TMP_InputField DatabaseName;
    public TMPro.TMP_InputField UserId;
    public TMPro.TMP_InputField UserPassword;
    public Button Connect;

    public void DatabaseConnect()
    {
   
    }

    void Start()
    {
        Connect.onClick.AddListener(DatabaseConnect);
    }

    void Update()
    {
        
    }
}
