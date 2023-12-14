using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreDetailTurbine : SceneManager
{

    public GameObject UI;

    public override void Enable()
    {
        UI.SetActive(true);
    }

    public override void Disable()
    {
        UI.SetActive(false);
    }


}
