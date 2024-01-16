using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraSceneMove : MonoBehaviour
{
    public List<CameraGroup> Cameras;
    public List<SceneManager> Callback;

    public CameraGroup GetCameras()
    {
        return Cameras[current];
    }

    int current = 0;
    public void SetCamera(int index, bool move)
    {
        index = index % Cameras.Count;
        if (current != index)
        {
            
            Cameras[index].enabled = true;
            Cameras[current].enabled = false;

            Callback[current].Disable(move);
            Callback[index].Enable(move);

            current = index;
        }
    }
    
    void Start()
    {
        Cameras[0].enabled = true;
        for (int i = 1; i < Cameras.Count; i++)
        {
            Cameras[i].enabled = false;
        }
    }

}
