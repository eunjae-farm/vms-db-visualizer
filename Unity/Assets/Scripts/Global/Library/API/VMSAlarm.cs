using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VMSAlarm
{
    [JsonProperty("date")]
    [field: SerializeField]
    public string Date{ get; set; }

    [JsonProperty("eu")]
    [field: SerializeField]
    public double Eu { get; set; }

    [JsonProperty("id")]
    [field: SerializeField]
    public int Id { get; set; }

    [JsonProperty("node")]
    [field: SerializeField]
    public int Node { get; set; }

    [JsonProperty("source")]
    [field: SerializeField]
    public int Source { get; set; }

    [JsonProperty("status")]
    [field: SerializeField]
    public int Status { get; set; }

    [JsonProperty("title")]
    [field: SerializeField]
    public string Title { get; set; }

    [JsonProperty("value")]
    [field: SerializeField]
    public double Value { get; set; }

    public string GetSource()
    {
        switch (this.Source)
        {
            case 0:
                return "F1";
            case 1:
                return "F2";
            case 2:
                return "F3";
            case 3:
                return "F4";
            case 4:
                return "F5";
            case 5:
                return "Diagnose";
            case 6:
                return "Overall";
            case 7:
                return "Band";
            case 8:
                return "CircleF1";
            case 9:
                return "CircleF2";
            case 10:
                return "ProteanIncrease";
            case 11:
                return "ProteanDecrease";
            default:
                throw new Exception("Get Source Error!");
        }
    }


    public static string GetStatus(int value)
    {
        switch (value)
        {
            case (1 << 0):
                return "Ok";
            case (1 << 1):
                return "NotMeasured";
            case (1 << 2):
                return "ProteanDecrease";
            case (1 << 3):
                return "ProteanIncrease";
            case (1 << 4):
                return "LACheckedNotUsed";
            case (1 << 5):
                return "HACheckedNotUsed";
            case (1 << 6):
                return "LW";
            case (1 << 7):
                return "HW";
            case (1 << 8):
                return "HA";
            case (1 << 9):
                return "LA";
            case (1 << 10):
                return "OutOfRange";
            case (1 << 11):
                return "CableFault";
            case (1 << 12):
                return "NotActive";
            case (1 << 13):
                return "CircleAlarm";
            case (1 << 14):
                return "CircleWarn";
            case (1 << 15):
                return "CircleAlarmChecked";
            case (1 << 16):
                return "CircleWarnChecked";
            case (1 << 17):
                return "TrendLongAlarm";
            case (1 << 18):
                return "Capture";
            case (1 << 19):
                return "TrendShortAlarm";
            case (1 << 20):
                return "ProtectionTrip";
            case (1 << 21):
                return "JumpALarm";
            case (1 << 22):
                return "JumpAlarmChecked";
            case (1 << 23):
                return "RelationAlarm";
            case (1 << 24):
                return "RelationAlarmChecked";
            case (1 << 25):
                return "DiagWarn";
            case (1 << 26):
                return "DiagAlarm";
            case (1 << 27):
                return "NoAlarmLevelsSet";
            case (1 << 28):
                return "Unstable";
            case (1 << 29):
                return "Transient";
            case (1 << 30):
                return "OutOfActiveRangeLOCK";
            default:
                return "Unknown";
        }
    }
    public string GetStatus()
    {
        return GetStatus(this.Status);
    }

}
