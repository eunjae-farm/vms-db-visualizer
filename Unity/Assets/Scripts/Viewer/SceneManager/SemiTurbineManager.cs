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
    public TMPro.TMP_Text InputCorrectPlacehorder;
    public TMPro.TMP_InputField InputError;
    public TMPro.TMP_Text InputErrorPlacehorder;

    private int index = -1;
    public void SetTurbineIndex(int i)
    {
        index = i;
        Recover();
    }
    
    public void ChangeOpen()
    {
        Panels.SetActive(!Panels.activeSelf);
    }

    public void Save()
    {
        TurbineConnectionDataManager.Instance.Save();
        InputCorrectPlacehorder.text = InputCorrect.text;
        InputErrorPlacehorder.text = InputError.text;
        UpdateForWindTurbine();
    }

    public void Recover()
    {
        TurbineConnectionDataManager.Instance.Load();
        UpdateForWindTurbine();
    }

    public void TextEditCorrect()
    {
        TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfCorrectForMotion = SliderCorrect.value;
        UpdateForWindTurbine();
    }

    public void TextEditError()
    {
        TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfErrorForMotion = SliderError.value;
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
        InputCorrect.text = co.ToString();
        InputError.text = err.ToString();
        SliderCorrect.value = co;
        SliderError.value = err;
        
        WindTurbine.MagnOfCorrect = co;
        WindTurbine.MagnOfError = err;
    }
}
