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
    public float CycleOfRefresh = 1f;
    private float CurrentCycleOfRefresh = 1f;
    
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
        
        bool checkAnyErrorisExist = StatusOfTurbine.Any(i => i.list.Any(p => p));
        StatusOfTurbine.Select(i => i.list.Any(p => p == true))
            .Select((item, idx) => (item, idx))
            .ToList()
            .ForEach(item =>
            {
                var convidx = new int[5] { 0, 1, 1, 2, 2, };
                var connvidx = convidx[item.idx];
                Bearing[connvidx].list.Select((o, i) => (o, i))
                    .ToList()
                    .ForEach(b =>
                {
                    var mr = b.o.GetComponentsInChildren<MeshRenderer>();
                    
                    if (checkAnyErrorisExist && item.item)
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
                            if (mr[i].material == DisabledTurbineObject)
                            {
                                continue;
                            }
                            mr[i].materials = AbledTurbineObject[connvidx][b.i][i];;
                        }
                    }
                });
            });

    }

    public void Awake()
    {
        // CreateData();
    }
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
    
    // Update is called once per frame
    void Update()
    {
        CurrentCycleOfRefresh += Time.deltaTime;

        if (CurrentCycleOfRefresh >= CycleOfRefresh)
        {
            CurrentCycleOfRefresh -= CycleOfRefresh * (int)(CurrentCycleOfRefresh / CycleOfRefresh);
            // CreateData();
        }

        if (ValueOfOverAll.Count == 0)
        {
            return;
        }

        var ratio = getNum(1 - (CurrentCycleOfRefresh / CycleOfRefresh));
        List<Vector3> vec = new List<Vector3>();

        for (int i = 0; i < Bearing.Count; i++)
        {
            var v = new Vector3(
                (float)(ValueOfOverAll[i].list[0] * ratio) * (StatusOfTurbine[i].list[0] ? MagnOfError : MagnOfCorrect),
                (float)(ValueOfOverAll[i].list[1] * ratio) * (StatusOfTurbine[i].list[0] ? MagnOfError : MagnOfCorrect),
                (float)(ValueOfOverAll[i].list[2] * ratio) * (StatusOfTurbine[i].list[0] ? MagnOfError : MagnOfCorrect)
            );
            vec.Add(v);
        }

        // foreach (var item in Bearing[0].list)
        // {
        //     var f = ComputeCenterPosition(FrontWing);
        //     item.transform.localPosition = vec[0];
        //     item.transform.localRotation = Quaternion.LookRotation(vec[0] - f);
        // }
        //
        // foreach (var item in Bearing[1].list)
        // {
        //     item.transform.localPosition = (vec[4] + vec[1]) / 2;
        //     item.transform.localRotation = Quaternion.LookRotation(vec[2] - vec[1]);
        // }
        //
        // foreach (var item in Bearing[2].list)
        // {
        //     item.transform.localPosition = (vec[4] + vec[3]) / 2;
        //     item.transform.localRotation = Quaternion.LookRotation(vec[4] - vec[3]);
        // }


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
                Bearing[group].list[l].transform.localPosition = vec[group];
            }
        }
    }

    public GameObject FrontWing;
    public GameObject SlowAsile;
    public GameObject FastAsile;
    
}
