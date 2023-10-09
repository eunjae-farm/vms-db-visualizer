using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTurbine : MonoBehaviour
{
    public GameObject Propeller;
    public GameObject Wing;
    public float WingRotatePerSeconds = 1;

    // Start is called before the first frame update
    void Start()
    {
        
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
        return (c, obj);
    }

    // Update is called once per frame
    void Update()
    {
        var (center, _) = ComputeCenterPosition(Wing);
        this.Wing.transform.RotateAround(center, Vector3.fwd, Time.deltaTime * WingRotatePerSeconds * 360);
        var cur = this.Propeller.transform.eulerAngles;
        this.Propeller.transform.RotateAround(Vector3.fwd, WingRotatePerSeconds * Time.deltaTime * 3.6f);
    }
}
