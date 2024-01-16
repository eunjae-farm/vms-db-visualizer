using System;
using System.Diagnostics;
using UnityEngine;

public abstract class SceneManager : MonoBehaviour
{
    public abstract void Enable(bool tomain);
    public abstract void Disable(bool tomain);

}
