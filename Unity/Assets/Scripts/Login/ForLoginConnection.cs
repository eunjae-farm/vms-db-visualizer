using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ForLoginConnection : MonoBehaviour
{
    public TMPro.TMP_InputField IpOfApiWithPort;
    public TMPro.TMP_InputField DatabaseIP;
    public TMPro.TMP_InputField DatabaseName;
    public TMPro.TMP_InputField UserId;
    public TMPro.TMP_InputField Password;

    private string GetPlaceholderText(TMPro.TMP_InputField field)
    {
        return field.placeholder.GetComponent<TMPro.TMP_Text> ().text.Split(" ")[1];
    }

    private string GetValue(TMPro.TMP_InputField field)
    {
        return field.text == "" ? GetPlaceholderText(field) : field.text;
    }

    private bool Validate(out LoginObject result)
    {
        result = new LoginObject();

        var ipPort = GetValue(IpOfApiWithPort).Split(":");
        var db = GetValue(DatabaseIP);
        var dbName = GetValue(DatabaseName);
        var id = GetValue(UserId);
        var pw = GetValue(Password);

        if (ipPort.Length == 2)
        {
            if (!int.TryParse(ipPort[1], out int apiPort))
            {
                return false;
            }

            result.IP = ipPort[0];
            result.Port = apiPort;
            result.DatabaseIP = db;
            result.DatabaseName = dbName;
            result.DatabaseId = id;
            result.DatabasePw = pw;
        }
        else
        {
            return false;
        }
        return true;
    }

    public void ProgramExit()
    {

    }
    public void DatabaseConnect()
    {
        if (Validate(out LoginObject result))
        {
            Server.Instance.Login(result);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Visualizer");
        }
        else
        {
            Debug.Log("ELSE!?");
        }
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
