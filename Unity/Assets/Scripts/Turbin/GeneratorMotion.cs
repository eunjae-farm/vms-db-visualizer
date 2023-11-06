using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorMotion : MonoBehaviour
{
    public GameObject outter_body;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
