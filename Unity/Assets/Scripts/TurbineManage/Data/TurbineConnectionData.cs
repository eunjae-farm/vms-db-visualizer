using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineConnectionData
{
    public string Name { get; set; }
    public string DBIP { get; set; }
    public string DBName { get; set; }
    public string ID { get; set; }
    public string PW { get; set; }
    public int NodeId { get; set; }
    public string NodeName { get; set; }

    public float WingRotatePerSeconds { get; set; }
    public float SlowRotateSpeed { get; set; }
    public float FastRotateSpeed { get; set; }
    public float MagnitudeForMotion { get; set; }
}
