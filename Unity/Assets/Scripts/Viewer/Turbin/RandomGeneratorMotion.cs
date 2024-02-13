using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomGeneratorMotion : MonoBehaviour
{
    public GameObject outter_body;
    public float RangeStart = -10;
    public float RangeEnd = 10;
    public float CycleOfRefresh = 1f;
    private float CurrentCycleOfRefresh = 1f;
    
    public float MagnOfCorrect = 0.5F;
    public float MagnOfError = 2.0F;

    // MainBearing, GearBox_MainBearing, GearBox_Generator, Generator

    public List<GameObject> VibrateWithMainBearing;
    public List<UnityList<GameObject>> Bearing;
    public List<UnityList<bool>> StatusOfTurbine;
    public List<Vector3> TargetOfPosition;
    public List<Vector3> OriginOfPosition;

    public Material DisabledTurbineObject;

    private List<List<List<Material>>> AbledTurbineObject;
    
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

    public void SetData(List<UnityList<bool>> status)
    {
        StatusOfTurbine = status;
        bool checkAnyErrorisExist =StatusOfTurbine.Any(i => i.list.Any(p => p));
        StatusOfTurbine.Select(i => i.list.Any(p => p == true))
            .Select((item, idx) => (item, idx))
            .ToList()
            .ForEach(item =>
            {
                Bearing[item.idx].list.Select((o, i) => (o, i))
                    .ToList()
                    .ForEach(b =>
                {
                    if (checkAnyErrorisExist && item.item)
                    {
                        b.o.GetComponentsInChildren<MeshRenderer>().ToList()
                            .ForEach(i =>
                            {
                                i.material = DisabledTurbineObject;
                            });
                    }
                    else
                    {
                        AbledTurbineObject[item.idx][b.i].Select((i, iidx) => (i, iidx))
                            .ToList().ForEach(data =>
                            {
                                b.o.GetComponentsInChildren<MeshRenderer>()[data.iidx].material = data.i;        
                            });
                        
                    }
                });
            });

    }

    public void Awake()
    {
        CreateData();
    }
    public void Start() { 
        foreach (var b in Bearing)
        {
            List<List<Material>> m = new List<List<Material>>();
            foreach (var obj in b.list)
            {
                var o = obj.AddComponent<Outline>();
                o.OutlineWidth = 7;
                o.enabled = false;
                o.OutlineColor = Color.red;
                m.Add(obj.GetComponentsInChildren<MeshRenderer>().Select((t => t.material)).ToList());
                AbledTurbineObject.Add(m);
            }
        }
    }

    void CreateData()
    {
        var arr = new List<List<Vector3>>
        {
            TargetOfPosition, OriginOfPosition
        };

        foreach (var obj in arr)
        {
            while (obj.Count < Bearing.Count)
            {
                obj.Add(new Vector3());
            }
        }


        for (int group = 0; group < Bearing.Count; group++)
        {
            // var o = Bearing[group].list[l];
            var vec = new Vector3(
                Random.Range(RangeStart, RangeEnd) * (StatusOfTurbine[group].list[0] ? MagnOfError : MagnOfCorrect),
                Random.Range(RangeStart, RangeEnd) * (StatusOfTurbine[group].list[1] ? MagnOfError : MagnOfCorrect),
                Random.Range(RangeStart, RangeEnd) * (StatusOfTurbine[group].list[2] ? MagnOfError : MagnOfCorrect)
            );

            TargetOfPosition[group] = vec;
            OriginOfPosition[group] = Bearing[group].list[0].transform.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CurrentCycleOfRefresh -= Time.deltaTime;
        
        if (CurrentCycleOfRefresh < 0)
        {
            CurrentCycleOfRefresh += CycleOfRefresh;
            CreateData();
        }

        var ratio = 1 - (CurrentCycleOfRefresh / CycleOfRefresh);
        for (int group = 0; group < Bearing.Count; group++)
        {
            if (group == 0)
            {
                for (int l = 0; l < VibrateWithMainBearing.Count; l++)
                {
                     VibrateWithMainBearing[l].transform.localPosition = Vector3.Lerp(
                         OriginOfPosition[group],
                         TargetOfPosition[group],
                         ratio);   
                }
            }
            for (int l = 0; l < Bearing[group].list.Count; l++)
            {
                Bearing[group].list[l].transform.localPosition = Vector3.Lerp(
                    OriginOfPosition[group],
                    TargetOfPosition[group],
                    ratio);
            }
        }
    }
}
