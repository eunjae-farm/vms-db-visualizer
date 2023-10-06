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
    public float WingRotatePerSeconds = 360;

    private float cur_refresh_tick = 0;
    private float cur_wing_rotate = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    //삼각형을 형성하는 세 점의 배열을 만듭니다.
    Vector3[] points = new Vector3[]
           {
            new Vector3(-1, -1, 0),
            new Vector3(1, -1, 0),
            new Vector3(0, 1, 0)
           };


    // Update is called once per frame
    void Update()
    {
        cur_refresh_tick += Time.deltaTime;
        cur_wing_rotate += Time.deltaTime;

        if (cur_refresh_tick > RefreshTick)
        {

        }
        if (cur_wing_rotate > WingRotatePerSeconds)
        {
            cur_wing_rotate -= WingRotatePerSeconds;
        }
        this.transform.localEulerAngles = new Vector3(0, 0, cur_wing_rotate * 360);
        
    }
}
