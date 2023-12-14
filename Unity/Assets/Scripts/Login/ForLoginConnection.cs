using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ForLoginConnection : MonoBehaviour
{
    public GameObject Exit;
    public PopupForAlarm Alarm;
    public TMPro.TMP_InputField FieldOfAPI;

    public void ProgramExit()
    {
        Exit.SetActive(true);
    }

    public void Connect()
    {
        if (FieldOfAPI.text == "")
        {
            UnityThread.executeInUpdate(() =>
            {
                Alarm.Open(PopupForAlarm.ButtonType.Error, "API의 주소 값을 공백으로 나타낼 수 없습니다.");
            });
            return;
        }

        var text = FieldOfAPI.text.Split(":");
        if (text.Length != 2)
        {
            UnityThread.executeInUpdate(() =>
            {
                Alarm.Open(PopupForAlarm.ButtonType.Error, "서버의 주소가 IP:Port 구조로 나타나야 합니다.");
            });
            return;
        }

        var ip = text[0];
        if (int.TryParse(text[1], out int port))
        {
            var (result, version) = Server.Instance.HealthyCheck(ip, port);
            if (result)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Visualizer");
            }
            else
            {
                UnityThread.executeInUpdate(() =>
                {
                    Alarm.Open(PopupForAlarm.ButtonType.Error, "서버와 통신에 실패하였습니다.");
                });
            }
        }
        else
        {
            UnityThread.executeInUpdate(() =>
            {
                Alarm.Open(PopupForAlarm.ButtonType.Error, "포트의 주소 값은 정수형 이어야 합니다.");
            });
        }
    }
}
