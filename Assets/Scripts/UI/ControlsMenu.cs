using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    public Button back;


    public void OnEnable()
    {
        back.Select();
    }
}

