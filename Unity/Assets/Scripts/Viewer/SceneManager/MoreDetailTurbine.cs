using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreDetailTurbine : SceneManager
{
    public GameObject UI;
    public CameraSceneMove Cameras;

    public override void Enable()
    {
        UI.SetActive(true);
    }

    public override void Disable()
    {
        UI.SetActive(false);
    }

    private void Update()
    {
        //마우스 클릭시
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = Input.mousePosition;
            var cameras = Cameras.GetCameras();

            foreach (var camera in cameras.Cameras)
            {
                var rect = camera.pixelRect;
                if (rect.Contains(clickPosition))
                {
                    Ray ray = camera.ScreenPointToRay(clickPosition);
                    if (Physics.Raycast(ray, out var raycastHit, 1000f))
                    {
                        if (raycastHit.transform != null)
                        {
                            Transform t = raycastHit.transform;

                            while (t.gameObject.name.Split("_")[0] != "00000") {
                                
                            }

                            Debug.Log(.gameObject.name);
                        }
                    }
                }
            }
        }
    }
}
