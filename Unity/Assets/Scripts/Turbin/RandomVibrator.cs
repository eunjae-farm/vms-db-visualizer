using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomVibrator : MonoBehaviour
{
    public GameObject FisrtBearing;
    public GameObject SecondBearing;
    public GameObject Generator;
    public GameObject BaseObject;
    public GameObject RedPivot;
    public GameObject Hole;
    public GameObject Body;
    public GameObject Electro;


    public float RefreshTick = 2;
    public float Amp = 5;

    private float cur_refresh_tick = 0;

    // Start is called before the first frame updatew
    void Start()
    {
        target_rot = new List<(float, float, GameObject)> 
        {
            (0, 0, FisrtBearing),
            (0, 0, SecondBearing),
            (0, 0, Generator),
            (0, 0, BaseObject),
            (0, 0, RedPivot),
            (0, 0, Hole),
            (0, 0, Body),
            (0, 0, Electro),
        };

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

    List<(float, float, GameObject)> target_rot = new List<(float, float, GameObject)>();


    // Update is called once per frame
    void Update()
    {
        cur_refresh_tick += Time.deltaTime;
        
        if (cur_refresh_tick > (1 / RefreshTick))
        {
            cur_refresh_tick = 0;
            for (int i = 0; i < target_rot.Count; i++) { 
                var amp = (Random.value * Amp) - (Amp / 2);
                //var cur = cur_rot[i];
                //var obj = shake[i].Item2;
                //var pos = shake[i].Item1;

                //obj.transform.RotateAround(pos, Vector3.fwd, amp - cur);
                //Debug.Log(target_rot[i].Item3.transform.rotation.z * 360 / Mathf.PI);
                target_rot[i] = new(target_rot[i].Item3.transform.rotation.z * 360 / Mathf.PI, amp, target_rot[i].Item3);
            }
        }

        for (int i = 0; i < target_rot.Count; i++)
        {
            var pos = ComputeCenterPosition(target_rot[i].Item3);
            var vari = (target_rot[i].Item2 - target_rot[i].Item1);
            //(1 / RefreshTick);
            var val = Time.deltaTime * RefreshTick * vari;
            target_rot[i].Item3.transform.RotateAround(pos, Vector3.forward, val);
        }

    }
}
