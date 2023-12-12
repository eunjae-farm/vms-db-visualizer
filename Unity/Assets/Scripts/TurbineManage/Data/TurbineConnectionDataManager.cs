using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TurbineConnectionDataManager : SingleTon<TurbineConnectionDataManager>
{
    public List<TurbineConnectionData> Data = new List<TurbineConnectionData>();

    public readonly string dataPath = Application.persistentDataPath;
    public readonly string fileName = "connection_information_for_vms.json";

    public void Save()
    {
        string json = JsonUtility.ToJson(Data);
        var path = Path.Combine(dataPath, fileName);
        File.WriteAllText(path, json);
    }

    public void Load()
    {
        Reset();
        var path = Path.Combine(dataPath, fileName);
        if (!File.Exists(path))
        {
            Save();
        }

        var data = File.ReadAllText(path);
        Data = JsonUtility.FromJson<List<TurbineConnectionData>>(data);
        
    }

    public void Reset()
    {
        var path = Path.Combine(dataPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        Save();
    }
}
