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
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(Data);
        var path = Path.Combine(dataPath, fileName);
        File.WriteAllText(path, json);
    }

    public void Load()
    {
        var path = Path.Combine(dataPath, fileName);
        if (!File.Exists(path))
        {
            Data.Add(new TurbineConnectionData
            {
                DBIP = "59.28.91.19",
                DBName = "Hangwon4",
                FastRotateSpeed = 1000,
                ID = "sa",
                PW = "skf1234!",
                MagnitudeOfCorrectForMotion = new List<float>
                {
                    0.5f,0.5f,0.5f,0.5f,0.5f
                },
                MagnitudeOfErrorForMotion = new List<float>
                {
                    2f,2f,2f,2f,2f
                },
                Name = "Ç×¿ø Ç³·Â",
                NodeId = 4,
                NodeName = "U88_H4",
                SlowRotateSpeed = 0.1f,
                ObserveBearing = new List<string>
                {
                    "MB_H_VEL",         "MB_V_VEL",         "MB_A_VEL",
                    "GB_H_Low_LVEL",    "GB_V_Low_LVEL",    "GB_A_Low_LVEL",
                    "GB_HSS_H_VEL",     "GB_HSS_V_VEL",     "GB_A_Low_LVEL",
                    "GE_RS_H_VEL",      "GE_RS_V_VEL",      "GE_RS_A_VEL",
                    "GE_GS_H_VEL",      "GE_GS_V_VEL",      "GE_RS_A_VEL"
                }
            });
            Save();
        }

        var data = File.ReadAllText(path);
        Data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TurbineConnectionData>>(data);
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
