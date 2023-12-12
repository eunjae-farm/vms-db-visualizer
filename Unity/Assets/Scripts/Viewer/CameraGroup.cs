using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraGroup
{
    public List<Camera> Cameras = new List<Camera>();

    private bool _enabled = false;
    public bool enabled {
        get
        {
            return _enabled;
        }
        set
        {
            foreach (var c in Cameras)
            {
                c.enabled = value;
            }
            _enabled = value;
        }
    }
}
