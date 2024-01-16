using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnityList<T> 
{   
    public UnityList()
    {
        list = new();
    }
    public List<T> list;
}
