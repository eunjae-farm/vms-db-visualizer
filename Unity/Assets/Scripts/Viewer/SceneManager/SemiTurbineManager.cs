using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class SemiTurbineManager : MonoBehaviour
{
    public RandomGeneratorMotion WindTurbine;
    
    public GameObject Panels;
    
    public List<Slider> SliderCorrect;
    public List<Slider> SliderError;
    public List<TMPro.TMP_InputField> InputCorrect;
    public List<TMPro.TMP_Text> InputCorrectPlacehorder;
    public List<TMPro.TMP_InputField> InputError;
    public List<TMPro.TMP_Text> InputErrorPlacehorder;

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
        for (int i = 0; i < 5; i++)
        {
            InputCorrectPlacehorder[i].text = InputCorrect[i].text;
            InputErrorPlacehorder[i].text = InputError[i].text;    
        }
        
        UpdateForWindTurbine();
    }

    public void Recover()
    {
        TurbineConnectionDataManager.Instance.Load();
        UpdateForWindTurbine();
    }

    public void ScrollEditCorrect(int i)
    {
        TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfCorrectForMotion[i] = SliderCorrect[i].value;
        UpdateForWindTurbine();
    }

    public void ScrollEditError(int i)
    {
        TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfErrorForMotion[i] = SliderError[i].value;
        UpdateForWindTurbine();
    }

    public void TextEditChangeCorrect(int i)
    {
        if (float.TryParse(InputCorrect[i].text, out float value))
        {
            TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfCorrectForMotion[i] = value;
            UpdateForWindTurbine();
        }
    }
    public void TextEditChangeError(int i)
    {
        if (float.TryParse(InputError[i].text, out float value))
        {
            TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfErrorForMotion[i] = value;
            UpdateForWindTurbine();
        }
    }

    public void UpdateForWindTurbine()
    {
        var co = TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfCorrectForMotion;
        var err = TurbineConnectionDataManager.Instance.Data[index].MagnitudeOfErrorForMotion;
        
        for (int i = 0; i < 5; i++)
        {
            InputCorrectPlacehorder[i].text = co[i].ToString();
            InputErrorPlacehorder[i].text = err[i].ToString();
            SliderCorrect[i].value = co[i];
            SliderError[i].value = err[i];
        }
        
        WindTurbine.MagnOfCorrect = co;
        WindTurbine.MagnOfError = err;
    }
}
