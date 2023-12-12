using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCode : MonoBehaviour
{
    public TMPro.TMP_Text NameAlias;
    public TMPro.TMP_Text NameTurbine;
    int index;
    string alias;
    string turbine;

    public event ElementOnClickEvent Click;

    public void OnClick()
    {
        Click?.Invoke(alias, turbine, index);
    }

    public void Setup(string alias_name, string turbine_name, int idx)
    {
        this.alias = alias_name;
        this.turbine = turbine_name;

        this.NameAlias.text = $"이름 : {alias_name}";
        this.NameTurbine.text = $"발전기 이름 : {turbine_name}";

        index = idx;
    }
}

public delegate void ElementOnClickEvent(string alias_name, string turbine_name, int idx);
