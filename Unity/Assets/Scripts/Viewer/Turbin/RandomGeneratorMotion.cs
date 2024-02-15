using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class RandomGeneratorMotion : MonoBehaviour
{
    public GameObject outter_body;
    // public float RangeStart = -10;
    // public float RangeEnd = 10;
    public float CycleOfRefreshX = 0.302858192f;
    public float CycleOfRefreshY = 0.41235f;
    public float CycleOfRefreshZ = 0.3561829f;
    private float CurrentCycleOfRefreshX = 0;
    private float CurrentCycleOfRefreshY = 0;
    private float CurrentCycleOfRefreshZ = 0;
    
    public float MagnOfCorrect = 0.5F;
    public float MagnOfError = 2.0F;

    // MainBearing, GearBox_MainBearing, GearBox_Generator, Generator

    public List<GameObject> VibrateWithMainBearing;
    public List<UnityList<GameObject>> Bearing;
    public List<UnityList<bool>> StatusOfTurbine;
    public List<UnityList<double>> ValueOfOverAll;
    // public List<Vector3> TargetOfPosition;
    // public List<Vector3> OriginOfPosition;

    public Material DisabledTurbineObject;

    private List<List<List<Material[]>>> AbledTurbineObject = new List<List<List<Material[]>>>();
    
    #region Removable Outter Body
    public void OutterBody(bool on)
    {
        if (on)
        {
            StartCoroutine(OutterBodyCoroutine(0.8f, 0, -9));
        }
        else
        {
            StartCoroutine(OutterBodyCoroutine(0.8f, -9, 0));
        }
        //outter_body.transform.position = new Vector3(0, 0, -9);
    }

    Vector3 get_curret_position(float z)
    {
        var p = outter_body.transform.localPosition;
        p.z = z;
        return p;
    }
    IEnumerator OutterBodyCoroutine(float duration, float s, float e)
    {

        float target = 0;
        outter_body.transform.localPosition = get_curret_position(s);
        while (target <= duration)
        {
            target += Time.deltaTime;
            var mag = target / duration;

            var pos = ((e - s) * mag) + s;

            outter_body.transform.localPosition = get_curret_position(pos);
            yield return new WaitForEndOfFrame();
        }
        outter_body.transform.localPosition = get_curret_position(e);
    }
    #endregion

    Vector3 ComputeCenterPosition(GameObject obj)
    {
        var center = obj.GetComponentsInChildren<Renderer>();
        Vector3 c = new Vector3();
        foreach (Renderer r in center)
        {
            c += r.bounds.center;
        }
        c /= center.Length;
        return c;
    }

    private Dictionary<Transform, Vector3> previousRot = new Dictionary<Transform, Vector3>();
    // void RotateObject(Transform objTransform, Vector3 rot)
    // {
    //     if (previousRot.ContainsKey(objTransform))
    //     {
    //         var prev = previousRot[objTransform];
    //         objTransform.RotateAround(ComputeCenterPosition(objTransform.gameObject), Vector3.forward, -prev.z);
    //         objTransform.RotateAround(ComputeCenterPosition(objTransform.gameObject), Vector3.up, -prev.y);
    //         objTransform.RotateAround(ComputeCenterPosition(objTransform.gameObject), Vector3.right, -prev.x);
    //     }
    //     
    //     objTransform.RotateAround(ComputeCenterPosition(objTransform.gameObject), Vector3.right, rot.x);
    //     objTransform.RotateAround(ComputeCenterPosition(objTransform.gameObject), Vector3.up, rot.y);
    //     objTransform.RotateAround(ComputeCenterPosition(objTransform.gameObject), Vector3.forward, rot.z);
    //     previousRot[objTransform] = rot;
    //     
    //     // objTransform.gameObject
    //     //     .GetComponentsInChildren<MeshRenderer>()
    //     //     .Select(i => i.gameObject.transform)
    //     //     .ToList()
    //     // .ForEach(i => i.localRotation = Quaternion.Euler(rot));
    // }
    
    public void OnOutline(int id, bool value)
    {
        for (int i = 0; i < Bearing.Count; i++)
        {
            foreach (var b in Bearing[i].list)
            {
                b.GetComponent<Outline>().enabled =((id == i) ? value : false);
            }
        }
    }

    public void SetData(List<UnityList<bool>> status, List<UnityList<double>> overall)
    {
        StatusOfTurbine = status;
        ValueOfOverAll = overall;

        for (int b = 0; b < Bearing.Count; b++)
        {
            for (int obj = 0; obj < Bearing[b].list.Count; obj++)
            {
                var mrs = Bearing[b].list[obj].GetComponentsInChildren<MeshRenderer>();
                for (int mr = 0; mr < mrs.Length; mr++)
                {
                    mrs[mr].materials = AbledTurbineObject[b][obj][mr];
                }
            }
        }
        
        bool checkAnyErrorisExist = StatusOfTurbine.Any(i => i.list.Any(p => p));
        var item = StatusOfTurbine.Select(i => i.list.Any(p => p == true)).ToList();
        for (int bear = 0; bear < Bearing.Count; bear++)
        {
            for (var mach = 0; mach < Bearing[bear].list.Count; mach++)
            {
                var mr = Bearing[bear].list[mach].GetComponentsInChildren<MeshRenderer>();
                if (checkAnyErrorisExist && !item[bear])
                {
                    for (int i = 0; i < mr.Length; i++)
                    {
                        mr[i].materials = Enumerable.Repeat(DisabledTurbineObject, mr[i].materials.Length).ToArray();
                    }
                }
                else
                {
                    for (int i = 0; i < mr.Length; i++)
                    {
                        if (mr[i].material.name.StartsWith(DisabledTurbineObject.name))
                        {
                            continue;
                        }

                        mr[i].materials = AbledTurbineObject[bear][mach][i];
                    }
                }
            }
        }

    }

    public void Awake()
    {
        // c1 = ComputeCenterPosition(Bearing[0].list[0]);
        // c2 = ComputeCenterPosition(Bearing[1].list[0]);
        // c3 = ComputeCenterPosition(Bearing[2].list[0]);
        // f = ComputeCenterPosition(FrontWing);
        // CreateData();
    }

    Vector3 mb  ;
    Vector3 con1;
    Vector3 gb  ;
    Vector3 con2;
    Vector3 ge  ;
    
    public void Start() {
        foreach (var b in Bearing)
        {
            List<List<Material[]>> m = new List<List<Material[]>>();
            foreach (var obj in b.list)
            {
                m.Add(obj.GetComponentsInChildren<MeshRenderer>().Select((t => t.materials)).ToList());
                
                if (obj.TryGetComponent<Outline>(out Outline _))
                {
                    continue;
                }
                var o = obj.AddComponent<Outline>();
                o.OutlineWidth = 7;
                o.enabled = false;
                o.OutlineColor = Color.red;
            }
            AbledTurbineObject.Add(m);
        }

        mb = new Vector3(0, 0, Bearing[0].list[0].transform.position.z);
       con1 = new Vector3(0, 0, SlowAsile.transform.position.z);
       gb = new Vector3(0, 0, Bearing[1].list[0].transform.position.z);
       con2 = new Vector3(0, 0, FastAsile.transform.position.z);
       ge = new Vector3(0, 0, Bearing[2].list[0].transform.position.z);
    }

    // void CreateData()
    // {
    //     var arr = new List<List<Vector3>>
    //     {
    //         TargetOfPosition, OriginOfPosition
    //     };
    //
    //     foreach (var obj in arr)
    //     {
    //         while (obj.Count < Bearing.Count)
    //         {
    //             obj.Add(new Vector3());
    //         }
    //     }
    //
    //
    //     for (int group = 0; group < Bearing.Count; group++)
    //     {
    //         // var o = Bearing[group].list[l];
    //         var vec = new Vector3(
    //             Random.Range(RangeStart, RangeEnd) * (StatusOfTurbine[group].list[0] ? MagnOfError : MagnOfCorrect),
    //             Random.Range(RangeStart, RangeEnd) * (StatusOfTurbine[group].list[1] ? MagnOfError : MagnOfCorrect),
    //             Random.Range(RangeStart, RangeEnd) * (StatusOfTurbine[group].list[2] ? MagnOfError : MagnOfCorrect)
    //         );
    //
    //         TargetOfPosition[group] = vec;
    //         OriginOfPosition[group] = Bearing[group].list[0].transform.localPosition;
    //     }
    // }

    double getNum(double d)
    {
        // 0 - 1 => -1 -> 1 -> -1
        // 0 1
        // 0 1 2 3 4
        // -2 -1 0 1 2
        // 2 1 0 1 2
        // 1 0 -1 0 1
        // 1 0 1 0 1
        // 2 0 2 0 2
        // 1 -1 1 -1 1
        var fr = Math.Abs((d * 4) - 2);
        var p = Math.Abs(fr - 1) * 2;
        return p - 1;
    }

    private float tttt = 0;

    private Vector3[] prevLocal = new Vector3[3] { new Vector3(), new Vector3(), new Vector3() };
    // Update is called once per frame
    void Update()
    {
        tttt += Time.deltaTime * 10;
        CurrentCycleOfRefreshX += Time.deltaTime;
        CurrentCycleOfRefreshY += Time.deltaTime;
        CurrentCycleOfRefreshZ += Time.deltaTime;
        
        if (CurrentCycleOfRefreshX >= CycleOfRefreshX)
        {
            CurrentCycleOfRefreshX -= CycleOfRefreshX * (int)(CurrentCycleOfRefreshX / CycleOfRefreshX);
        }
        if (CurrentCycleOfRefreshY >= CycleOfRefreshY)
        {
            CurrentCycleOfRefreshY -= CycleOfRefreshY * (int)(CurrentCycleOfRefreshY / CycleOfRefreshY);
        }
        if (CurrentCycleOfRefreshZ >= CycleOfRefreshZ)
        {
            CurrentCycleOfRefreshZ -= CycleOfRefreshZ * (int)(CurrentCycleOfRefreshZ / CycleOfRefreshZ);
        }

        if (ValueOfOverAll.Count == 0)
        {
            return;
        }

        var ratioX = getNum(1 - (CurrentCycleOfRefreshX / CycleOfRefreshX));
        var ratioY = getNum(1 - (CurrentCycleOfRefreshY / CycleOfRefreshY));
        var ratioZ = getNum(1 - (CurrentCycleOfRefreshZ / CycleOfRefreshZ));
        List<Vector3> vec = new List<Vector3>();

        for (int i = 0; i < 5; i++)
        {
            // m to mm
            var v = new Vector3(
                (float)(ValueOfOverAll[i].list[0] * ratioX) * ((StatusOfTurbine[i].list[0] ? MagnOfError : MagnOfCorrect) / 1000),
                (float)(ValueOfOverAll[i].list[1] * ratioY) * ((StatusOfTurbine[i].list[1] ? MagnOfError : MagnOfCorrect) / 1000),
                (float)(ValueOfOverAll[i].list[2] * ratioZ) * ((StatusOfTurbine[i].list[2] ? MagnOfError : MagnOfCorrect) / 1000)
            );
            vec.Add(v);
        }

        var p0 = FrontWing.transform.position;
        p0.x = p0.y = 0;
        
        var p1 = vec[0] + mb;
        var p2 = ((vec[1] + vec[2]) / 2) + gb;
        var p3 = ((vec[3] + vec[4]) / 2) + ge;
        
        
        // mb  
        //     con1
        // gb  
        //     con2
        // ge  
        
        var pc1 = p1 - p0;
        // var pd0 = SlowAsile.transform.position - Bearing[0].list[0].transform.position;
        
        // var pc2 = Bearing[1].list[0].transform.position - SlowAsile.transform.position;
        var pd1 = p2 - p1;
        // var pc3 = FastAsile.transform.position - Bearing[1].list[0].transform.position;

        var pd2 = p3 - p2;
        // SlowAsile.transform.localRotation = Quaternion.LookRotation(pc2);
        // FastAsile.transform.localRotation = Quaternion.LookRotation(pc3);
        
        foreach (var item in Bearing[0].list)
        {
            var p = Quaternion.LookRotation(-pc1).eulerAngles * 10;
            p.z = 0;
            item.transform.localPosition = vec[0];
            item.transform.localRotation = Quaternion.Euler(p);
            // prevLocal[0] = vec[0];
        }
        
        foreach (var item in Bearing[1].list)
        {
            var p = Quaternion.LookRotation(-pd1).eulerAngles;
            p.z = 0;
            item.transform.localRotation = Quaternion.Euler(p);
            // RotateObject(item.transform, Quaternion.LookRotation((c2 + vec[2]) - (c1 + vec[1])).eulerAngles);
            item.transform.localPosition =  vec[1];
            // prevLocal[1] = vec[1];
        }
        
        foreach (var item in Bearing[2].list)
        {
            var p = Quaternion.LookRotation(-pd2).eulerAngles;
            p.z = 0;
            item.transform.localRotation = Quaternion.Euler(p);
            // RotateObject(item.transform, Quaternion.LookRotation((c3 + vec[4]) - (c2 + vec[3])).eulerAngles);
            item.transform.localPosition = vec[2];
            // prevLocal[2] = vec[2];
        }
        //
        // RotateObject(Bearing[0].list[0].transform, Quaternion.LookRotation(c2 - c1).eulerAngles);
        // RotateObject(Bearing[2].list[0].transform, Quaternion.LookRotation(c3 - c2).eulerAngles);
        // Quaternion.LookRotation(c3 - c2).eulerAngles;
        
        for (int group = 0; group < 3; group++)
        {
            if (group == 0)
            {
                for (int l = 0; l < VibrateWithMainBearing.Count; l++)
                {
                    VibrateWithMainBearing[l].transform.localPosition = vec[group]; 
                }
            }

            for (int l = 0; l < Bearing[group].list.Count; l++)
            {
                // RotateObject(Bearing[group].list[l].transform, new Vector3(10,15,30));
                // Bearing[group].list[l].transform.localPosition = vec[group];
            }
        }
    }

    public GameObject FrontWing;
    public GameObject SlowAsile;
    public GameObject FastAsile;
    
}
