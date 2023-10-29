using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraSceneMove : MonoBehaviour
{
    public List<Camera> Cameras;

    int current = 0;
    public void SetCamera(int index)
    {
        index = index % Cameras.Count;
        if (current != index)
        {
            Cameras[index].enabled = true;
            Cameras[current].enabled = false;
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            SetCamera(0);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            SetCamera(1);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            SetCamera(2);
        }
        else if (Input.GetKey(KeyCode.R))
        {
            SetCamera(3);
        }
        else if (Input.GetKey(KeyCode.T))
        {
            SetCamera(4);
        }
        else if (Input.GetKey(KeyCode.Y))
        {
            SetCamera(5);
        }
    }
}
