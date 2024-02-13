using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SemiTurbineManager : MonoBehaviour
{
    public RandomGeneratorMotion WindTurbine;
    public Slider SliderCorrect;
    public Slider SliderError;
    public GameObject Panels;
    public TMPro.TMP_InputField InputCorrect;
    public TMPro.TMP_InputField InputError;

    private int index = -1;
    public void SetTurbineIndex(int i)
    {
        index = i;
        InputCorrect.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text =
            TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfCorrectForMotion.ToString();
        InputError.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text =
            TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfErrorForMotion.ToString();
    }
    
    public void ChangeOpen()
    {
        Panels.SetActive(!Panels.activeSelf);
    }

    public void Save()
    {
        TurbineConnectionDataManager.Instance.Save();
        InputCorrect.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = InputCorrect.text;
        InputError.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = InputError.text;
        UpdateForWindTurbine();
    }

    public void Recover()
    {
        TurbineConnectionDataManager.Instance.Load();
        SetTurbineIndex(index);
    }

    public void TextEditCorrect()
    {
        TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfCorrectForMotion = SliderCorrect.value;
        InputCorrect.text = SliderCorrect.value.ToString();
        UpdateForWindTurbine();
    }

    public void TextEditError()
    {
        TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfErrorForMotion = SliderError.value;
        InputError.text = SliderError.value.ToString();
        UpdateForWindTurbine();
    }

    public void TextEditChangeCorrect()
    {
        if (float.TryParse(InputCorrect.text, out float value))
        {
            TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfCorrectForMotion = value;
            UpdateForWindTurbine();
        }
    }
    public void TextEditChangeError()
    {
        if (float.TryParse(InputError.text, out float value))
        {
            TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfErrorForMotion = value;
            UpdateForWindTurbine();
        }
    }

    public void UpdateForWindTurbine()
    {
        var co = TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfCorrectForMotion;
        var err = TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfErrorForMotion;
        WindTurbine.MagnOfCorrect = co;
        WindTurbine.MagnOfCorrect = err;
    }
}
