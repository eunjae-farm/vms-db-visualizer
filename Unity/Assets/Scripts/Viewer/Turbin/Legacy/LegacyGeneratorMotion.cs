using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LegacyGeneratorMotion : MonoBehaviour
{
    public GameObject outter_body;
    public float Magn = 5.0F;

    // MainBearing, GearBox_MainBearing, GearBox_Generator, Generator
    
    public List<UnityList<GameObject>> Bearing; 

    public List<UnityList<float>> Times;
    public List<UnityList<NodeData>> Nodes;


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
            foreach (var b in Bearing[id].list)
            {
                b.GetComponent<Outline>().enabled =((id == i) ? value : false);
            }
        }
    }

    public void Awake()
    {
        foreach (var b in Bearing)
        {
            foreach (var obj in b.list)
            {
                var o = obj.AddComponent<Outline>();
                o.OutlineWidth = 7;
                o.enabled = false;
            }
        }
    }
        
    public List<UnityList<NodeData>> CurrentConvetedData()
    {
        return Nodes;
    }

    public List<UnityList<NodeData>> ConvertData(List<NodeData> data, List<string> bearingSequnence)
    {
        var GE_GS_A_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[0]);
        var GE_GS_H_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[1]);
        var GE_GS_V_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[2]);

        var GE_RS_A_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[3]);
        var GE_RS_H_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[4]);
        var GE_RS_V_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[5]);

        var MB_A_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[6]);
        var MB_H_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[7]);
        var MB_V_VEL = data.FirstOrDefault(item => item.Node.Name == bearingSequnence[8]);

        Nodes.Clear();
        Nodes.Add(new UnityList<NodeData> { list = new List<NodeData> { MB_A_VEL, MB_H_VEL, MB_V_VEL } });
        Nodes.Add(new UnityList<NodeData> { list = new List<NodeData> { GE_RS_A_VEL, GE_RS_H_VEL, GE_RS_V_VEL } });
        Nodes.Add(new UnityList<NodeData> { list = new List<NodeData> { GE_GS_A_VEL, GE_GS_H_VEL, GE_GS_V_VEL } });

        return Nodes;
    }

    
    // r
    public void SetData(List<NodeData> data, List<string> bearingSequnence)
    {
        // MainBearing, GearBox_MainBearing, GearBox_Generator, Generator
        Times.Clear();
        Times.Add(new UnityList<float> { list = new List<float> { 0, 0, 0 } });
        Times.Add(new UnityList<float> { list = new List<float> { 0, 0, 0 } });
        Times.Add(new UnityList<float> { list = new List<float> { 0, 0, 0 } });

        ConvertData(data, bearingSequnence);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Nodes.Count == 0)
        {
            return;
        }

        for (int group = 0; group < Bearing.Count; group++)
        {
            Vector3 vec = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                if (Nodes[group].list[i] == null)
                {
                    vec[i] = 0;
                    continue;
                }
                Times[group].list[i] += Time.deltaTime;
                if (Times[group].list[i] > Nodes[group].list[i].Chart.Duration)
                {
                    Times[group].list[i] -= Nodes[group].list[i].Chart.Duration;
                }
                int idx = (int)((Times[group].list[i] / Nodes[group].list[i].Chart.Duration) * Nodes[group].list[i].Chart.Data.Length);
                idx = Mathf.Min(idx, Nodes[group].list[i].Chart.Data.Length);
                vec[i] = (float)(Nodes[group].list[i].Chart.Data[idx] * Magn);
            }
            
            for (int bearing = 0; bearing < Bearing[group].list.Count; bearing++)
            {
                Bearing[group].list[bearing].transform.localPosition = vec;
            }
        }
    }
}
