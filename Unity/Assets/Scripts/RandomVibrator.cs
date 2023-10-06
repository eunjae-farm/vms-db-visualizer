using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomVibrator : MonoBehaviour
{
    public GameObject Wing;
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
    public float WingRotatePerSeconds = 1;

    private float cur_refresh_tick = 0;

    // Start is called before the first frame updatew
    void Start()
    {
        cur_rot = new List<float>
        {
            0,0,0,0,0,0,0,0,
        };
    }
    (Vector3, GameObject) ComputeCenterPosition(GameObject obj)
    {
        var center = Wing.GetComponentsInChildren<Renderer>();
        Vector3 c = new Vector3();
        foreach (Renderer r in center)
        {
            c += r.bounds.center;
        }
        c /= center.Length;
        return (c,  obj);
    }

    bool v = false;
    List<float> cur_rot = new List<float>();


    // Update is called once per frame
    void Update()
    {
        cur_refresh_tick += Time.deltaTime;
        
        var w = ComputeCenterPosition(Wing);
        this.Wing.transform.RotateAround(w.Item1, Vector3.fwd, Time.deltaTime * WingRotatePerSeconds * 360);

        if (cur_refresh_tick > (1 / RefreshTick))
        {
            cur_refresh_tick = 0;
            v = !v;

            var fb = ComputeCenterPosition(FisrtBearing);
            var sb = ComputeCenterPosition(SecondBearing);
            var g = ComputeCenterPosition(Generator);
            var bo = ComputeCenterPosition(BaseObject);
            var rp = ComputeCenterPosition(RedPivot);
            //var h = ComputeCenterPosition(Hole);
            var b = ComputeCenterPosition(Body);
            var e = ComputeCenterPosition(Electro);

            var shake = new List<(Vector3, GameObject)>
            {
                fb,sb,g,bo,rp,b,e,
            };

            for (int i = 0; i < shake.Count; i++) { 
                var amp = (Random.value * Amp) - (Amp / 2);
                var cur = cur_rot[i];
                var obj = shake[i].Item2;
                var pos = shake[i].Item1;

                obj.transform.RotateAround(pos, Vector3.fwd, amp - cur);
                cur_rot[i] = amp;
            }

        }

    }
}
